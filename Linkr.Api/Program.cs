using Asp.Versioning;
using Serilog;
using Linkr.Api.Extensions;
using Linkr.Api.Middleware;
using Linkr.Domain;
using Linkr.Infrastructure;
using System.Text.Json.Serialization;

namespace Linkr.Api
{
	public class Program
	{
		private const string CorsPolicy = "CorsPolicy";

		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var env = builder.Environment;

			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateBootstrapLogger();

			var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process) ??
						  Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.User) ??
						  Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Machine);

			builder.Configuration.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{envName}.json", optional: true)
				.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true);

			// Use Kestrel as the web server
			builder.WebHost.UseKestrel();

			// Add CORS
			builder.Services.ConfigureCors();

			// Add Postgres
			if (env.IsProduction())
			{
				builder.Configuration.ConfigureRailwayDatabaseConnectionString();
			}
			builder.Services.AddDatabase(builder.Configuration);

			// Add services
			builder.Services.AddDomain();
			builder.Services.AddInfrastructure(builder.Configuration);

			// Add Controllers
			builder.Services.AddControllers()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				});

			// Add rate limiting
			builder.Services.AddRateLimiting();

			// add Api Versioning
			builder.Services.AddApiVersioning(options =>
			{
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ReportApiVersions = true;
				options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
					new HeaderApiVersionReader("X-Api-Version"));
			}).AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
			});

			// Serilog configuration
			builder.Host.UseSerilog((context, config) =>
			{
				config.ReadFrom.Configuration(context.Configuration);
			});

			// Add Swagger
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.UseMiddleware<ExceptionMiddleware>();

			app.UseCors(CorsPolicy);

			app.MapControllers();

			app.Run();
		}
	}
}