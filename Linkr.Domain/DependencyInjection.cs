using FluentValidation;
using System.Reflection;
using Linkr.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Linkr.Domain;

public static class DependencyInjection
{
	public static IServiceCollection AddDomain(this IServiceCollection services)
	{
		services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

		services.AddScoped<IUrlService, UrlService>();
		services.AddScoped<IUserService, UserService>();
		return services;
	}
}