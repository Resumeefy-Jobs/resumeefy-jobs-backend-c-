using Hangfire;
using Hangfire.PostgreSql;
using Npgsql;
using Resumeefy.API.Middlewares;
using Resumeefy.Application.Features.Auth.Commands.Register;
using Resumeefy.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

var connString = BuildConnectionString(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

builder.Services.AddHangfire(configuration => configuration
	.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
	.UseSimpleAssemblyNameTypeSerializer()
	.UseRecommendedSerializerSettings()
	.UsePostgreSqlStorage(options =>
	{
		options.UseNpgsqlConnection(builder.Configuration.GetConnectionString(connString));
	}));

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

app.Run();
