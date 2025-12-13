using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Resumeefy.Core.Common;
using Resumeefy.Core.Exceptions;
using Resumeefy.Core.Interfaces;
using System.Text;

namespace Resumeefy.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, BaseResponse<string>>
{
	private readonly IUserRepository _userRepository;

	public VerifyEmailHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<BaseResponse<string>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
	{
		Guid userId;
		string token;

		try
		{
			byte[] bytes = WebEncoders.Base64UrlDecode(request.Code);
			string decodedString = Encoding.UTF8.GetString(bytes);

			var parts = decodedString.Split(':');

			if (parts.Length != 2) throw new Exception();

			userId = Guid.Parse(parts[0]);
			token = parts[1];
		}
		catch
		{
			throw new ApiException("Invalid verification link.");
		}

		var user = await _userRepository.GetByIdAsync(userId);

		if (user == null) throw new NotFoundException("User", userId);

		if (user.IsEmailVerified)
		{
			return new BaseResponse<string>("Email is already verified.");
		}

		if (user.VerificationToken != token)
		{
			throw new ApiException("Invalid verification token.");
		}

		user.IsEmailVerified = true;
		user.VerificationToken = null;

		await _userRepository.UpdateAsync(user);

		return new BaseResponse<string>("Email verified successfully. You can now login.", "Email Verified");
	}
}