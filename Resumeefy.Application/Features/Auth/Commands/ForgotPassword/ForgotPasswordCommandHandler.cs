using Hangfire;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Resumeefy.Core.Common;
using Resumeefy.Core.Interfaces;
using System.Text;

namespace Resumeefy.Application.Features.Auth.Commands.ForgotPassword
{
	public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, BaseResponse<string>>
	{
		private readonly IUserRepository _userRepository;
		private readonly IBackgroundJobClient _backgroundJobClient;
		private readonly IConfiguration _configuration;

		public ForgotPasswordHandler(IUserRepository userRepository, IBackgroundJobClient backgroundJobClient, IConfiguration configuration)
		{
			_userRepository = userRepository;
			_backgroundJobClient = backgroundJobClient;
			_configuration = configuration;
		}

		public async Task<BaseResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.GetByEmailAsync(request.Email);

			if (user == null)
			{
				return new BaseResponse<string>("If your email exists, a reset link has been sent.");
			}

			user.PasswordResetToken = Guid.NewGuid().ToString("N");
			user.TokenExpiresAt = DateTime.UtcNow.AddHours(1);
			user.UpdatedAt = DateTime.UtcNow;

			await _userRepository.UpdateAsync(user);

			string payload = $"{user.Email}:{user.PasswordResetToken}";
			byte[] bytes = Encoding.UTF8.GetBytes(payload);
			string code = WebEncoders.Base64UrlEncode(bytes);

			var port = _configuration["HostingPort"] ?? Environment.GetEnvironmentVariable("HostingPort");
			string resetUrl = $"http://localhost:{port}/api/auth/reset-password?code={code}";

			string emailBody = $@"
            <h3>Reset Your Password</h3>
            <p>You requested a password reset. Click the link below to set a new password:</p>
            <p><a href='{resetUrl}'>Reset Password</a></p>
            <p>This link expires in 1 hour.</p>";

			_backgroundJobClient.Enqueue<IEmailService>(email =>
				email.SendEmailAsync(user.Email, "Reset Your Password", emailBody));

			return new BaseResponse<string>("If your email exists, a reset link has been sent.");
		}
	}
}
