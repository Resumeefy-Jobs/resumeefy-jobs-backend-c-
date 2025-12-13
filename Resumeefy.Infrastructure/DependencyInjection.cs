using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resumeefy.Application.Interfaces;
using Resumeefy.Core.Interfaces;
using Resumeefy.Infrastructure.Data;
using Resumeefy.Infrastructure.Repositories;
using Resumeefy.Infrastructure.Services;

namespace Resumeefy.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");

		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseNpgsql(connectionString));

		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		services.AddTransient<IEmailService, EmailService>();
		services.AddScoped<IGoogleAuthService, GoogleAuthService>();

		return services;
	}
}