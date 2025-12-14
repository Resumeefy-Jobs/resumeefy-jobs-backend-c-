using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
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
		string BuildConnectionString(IConfiguration cfg)
		{
			var host = Environment.GetEnvironmentVariable("DB_HOST");
			var db = Environment.GetEnvironmentVariable("DB_NAME");
			var user = Environment.GetEnvironmentVariable("DB_USERNAME");
			var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
			var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

			if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(db) &&
				!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
			{
				var csb = new NpgsqlConnectionStringBuilder
				{
					Host = host,
					Port = int.Parse(port),
					Username = user,
					Password = password,
					Database = db,
					SslMode = SslMode.Require
				};
				return csb.ToString();
			}
			return cfg.GetConnectionString("DefaultConnection") ?? string.Empty;
		}

		var connString = BuildConnectionString(configuration);

		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseNpgsql(connString));

		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		services.AddTransient<IEmailService, EmailService>();
		services.AddScoped<IGoogleAuthService, GoogleAuthService>();

		return services;
	}
}