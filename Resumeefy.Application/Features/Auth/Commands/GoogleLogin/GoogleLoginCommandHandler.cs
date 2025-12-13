using MediatR;
using Resumeefy.Application.DTOs;
using Resumeefy.Application.Interfaces;
using Resumeefy.Core.Common;
using Resumeefy.Core.Entities;
using Resumeefy.Core.Interfaces;

namespace Resumeefy.Application.Features.Auth.Commands.GoogleLogin
{
	public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, BaseResponse<AuthResponseDto>>
	{
		private readonly IUserRepository _userRepository;
		private readonly IGoogleAuthService _googleAuthService;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;

		public GoogleLoginHandler(
			IUserRepository userRepository,
			IGoogleAuthService googleAuthService,
			IJwtTokenGenerator jwtTokenGenerator)
		{
			_userRepository = userRepository;
			_googleAuthService = googleAuthService;
			_jwtTokenGenerator = jwtTokenGenerator;
		}

		public async Task<BaseResponse<AuthResponseDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
		{
			var googleUser = await _googleAuthService.ValidateAsync(request.IdToken);

			var user = await _userRepository.GetByEmailAsync(googleUser.Email);

			if (user == null)
			{
				user = new User
				{
					Email = googleUser.Email,
					Role = request.Role,
					IsEmailVerified = true,
					PasswordHash = "", // No password for OAuth users// Note: You might want to save FirstName/LastName/Picture to Profile here
				};

				if (request.Role == Core.Enums.UserRole.JobSeeker)
					user.JobSeekerProfile = new JobSeekerProfile
					{
						UserId = user.Id
					}; // Add Name/Picture here later
				else
					user.CompanyProfile = new CompanyProfile { UserId = user.Id };

				await _userRepository.AddAsync(user);
			}

			// 4. Generate JWT
			var token = _jwtTokenGenerator.GenerateToken(user);

			var responseDto = new AuthResponseDto
			{
				Id = user.Id,
				Email = user.Email,
				Role = user.Role.ToString(),
				Token = token
			};

			return new BaseResponse<AuthResponseDto>(responseDto, "Google Login Successful");
		}
	}
}
