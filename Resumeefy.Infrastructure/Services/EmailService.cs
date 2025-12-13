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
	private readonly ILogger<EmailService> _logger; // Add Logger

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
			email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;

			var builder = new BodyBuilder { HtmlBody = body };
			email.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();

			await smtp.ConnectAsync(
				_configuration["EmailSettings:Host"],
				int.Parse(_configuration["EmailSettings:Port"]!),
				SecureSocketOptions.StartTls
			);

			await smtp.AuthenticateAsync(
				_configuration["EmailSettings:User"],
				_configuration["EmailSettings:Password"]
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