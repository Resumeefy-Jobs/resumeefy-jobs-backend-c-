using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Resumeefy.Core.Interfaces;

namespace Resumeefy.Infrastructure.Services;

public class EmailService : IEmailService
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<EmailService> _logger;

	public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public async Task SendEmailAsync(string to, string subject, string body)
	{
		try
		{
			_logger.LogInformation($"Attempting to send email to {to}...");

			var email = new MimeMessage();
			var fromAddress = _configuration["EmailSettings:From"] ?? Environment.GetEnvironmentVariable("EmailSettings__From");
			email.From.Add(MailboxAddress.Parse(fromAddress));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;

			var builder = new BodyBuilder { HtmlBody = body };
			email.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();
			var emailHost = _configuration["EmailSettings:Host"] ?? Environment.GetEnvironmentVariable("EmailSettings__Host");
			var emailPort = _configuration["EmailSettings:Port"] ?? Environment.GetEnvironmentVariable("EmailSettings__Port");
			await smtp.ConnectAsync(
				emailHost,
				int.Parse(emailPort!),
				SecureSocketOptions.StartTls
			);

			var emailUser = _configuration["EmailSettings:User"] ?? Environment.GetEnvironmentVariable("EmailSettings__User");
			var emailPassword = _configuration["EmailSettings:Password"] ?? Environment.GetEnvironmentVariable("EmailSettings__Password");
			await smtp.AuthenticateAsync(
				emailUser,
				emailPassword
			);

			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);

			_logger.LogInformation($"✅ Email successfully sent to {to}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"❌ FAILED to send email to {to}. Error: {ex.Message}");
			throw;
		}
	}
}