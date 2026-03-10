using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Linkr.Infrastructure.Data;
using System.Threading.RateLimiting;

namespace Linkr.Api.Extensions;

public static class ServiceExtension
{
	public static void ConfigureCors(this IServiceCollection services) =>
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", builder =>
				builder.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());
			});

	public static void AddRateLimiting(this IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
			options.AddFixedWindowLimiter("fixed", limiterOptions =>
			{
				limiterOptions.PermitLimit = 50;
				limiterOptions.Window = TimeSpan.FromMinutes(1);
				limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				limiterOptions.QueueLimit = 5;
			});
		});
	}

	public static void ConfigureRailwayDatabaseConnectionString(this IConfiguration configuration)
	{
		var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
		if (string.IsNullOrEmpty(databaseUrl))
		{
			throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
		}
		var databaseUri = new Uri(databaseUrl);
		var userInfo = databaseUri.UserInfo.Split(':');
		var builder = new Npgsql.NpgsqlConnectionStringBuilder
		{
			Host = databaseUri.Host,
			Port = databaseUri.Port,
			Username = userInfo[0],
			Password = userInfo[1],
			Database = databaseUri.AbsolutePath.TrimStart('/'),
			SslMode = Npgsql.SslMode.Require,
		};
		configuration.GetSection("ConnectionStrings")["DefaultConnection"] = builder.ToString();
	}

	public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ShotiContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), serverOptions =>
			{
				serverOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
			});
		});
		return services;
	}
}
