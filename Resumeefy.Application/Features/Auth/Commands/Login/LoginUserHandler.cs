using MediatR;
using Resumeefy.Application.DTOs;
using Resumeefy.Application.Interfaces;
using Resumeefy.Core.Common;
using Resumeefy.Core.Exceptions;
using Resumeefy.Core.Interfaces;

namespace Resumeefy.Application.Features.Auth.Commands.Login;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, BaseResponse<AuthResponseDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;

	public LoginUserHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
	{
		_userRepository = userRepository;
		_jwtTokenGenerator = jwtTokenGenerator;
	}

	public async Task<BaseResponse<AuthResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByEmailAsync(request.Email);

		if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
		{
			throw new ApiException("Invalid Email or Password.");
		}

		var token = _jwtTokenGenerator.GenerateToken(user);

		var responseDto = new AuthResponseDto
		{
			Id = user.Id,
			Email = user.Email,
			Role = user.Role.ToString(),
			Token = token
		};

		return new BaseResponse<AuthResponseDto>(responseDto, "Login Successful");
	}
}