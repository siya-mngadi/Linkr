using System.Text;
using Linkr.Domain.Entities;
using Linkr.Domain.Repositories;
using Linkr.Infrastructure.Data;
using Linkr.Domain.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Linkr.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Linkr.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Linkr.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddConfiguration(configuration);
		services.AddRepositories();
		services.AddTokenAuthentication(configuration);
		return services;
	}

	public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)));
		services.Configure<ShortCodeSettings>(configuration.GetSection(nameof(ShortCodeSettings)));
		services.Configure<WebsiteConfiguration>(configuration.GetSection(nameof(WebsiteConfiguration)));
		return services;
	}

	public static IServiceCollection AddRepositories(this IServiceCollection services)
	{
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IUrlRepository, UrlRepository>();
		services.AddScoped<IUrlClickRepository, UrlClickRepository>();
		return services;
	}

	public static IServiceCollection AddTokenAuthentication(this IServiceCollection services,
		IConfiguration configuration)
	{
		var settings = configuration.GetSection(nameof(AuthenticationSettings));
		var settingsTyped = settings.Get<AuthenticationSettings>();
		services.Configure<AuthenticationSettings>(settings);
		var key = Encoding.ASCII.GetBytes(settingsTyped.Secret);
		services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ShotiContext>();
		services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(x =>
		{
			x.TokenValidationParameters = new TokenValidationParameters
			{
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false,
			};
		});
		return services;
	}
}