using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Resumeefy.Core.Common;
using Resumeefy.Core.Exceptions;
using Resumeefy.Core.Interfaces;
using System.Text;

namespace Resumeefy.Application.Features.Auth.Commands.ResetPassword
{
	public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, BaseResponse<string>>
	{
		private readonly IUserRepository _userRepository;

		public ResetPasswordHandler(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<BaseResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
		{
			string email;
			string token;

			try
			{
				byte[] bytes = WebEncoders.Base64UrlDecode(request.Code);
				string payload = Encoding.UTF8.GetString(bytes);
				var parts = payload.Split(':');
				email = parts[0];
				token = parts[1];
			}
			catch
			{
				throw new ApiException("Invalid or corrupted reset link.");
			}

			var user = await _userRepository.GetByEmailAsync(email);
			if (user == null) throw new ApiException("Invalid request.");

			if (user.PasswordResetToken != token)
			{
				throw new ApiException("Invalid token.");
			}

			if (user.TokenExpiresAt < DateTime.UtcNow)
			{
				throw new ApiException("Reset link has expired. Please request a new one.");
			}

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

			user.PasswordResetToken = null;
			user.TokenExpiresAt = null;

			await _userRepository.UpdateAsync(user);

			return new BaseResponse<string>("Password reset successfully. You can now login with your new password.");
		}
	}
}
