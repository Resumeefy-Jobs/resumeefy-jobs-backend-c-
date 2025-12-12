using MediatR;
using Resumeefy.Core.Common;
using Resumeefy.Core.Entities;
using Resumeefy.Core.Enums;
using Resumeefy.Core.Exceptions; // Use our new exceptions
using Resumeefy.Core.Interfaces;

namespace Resumeefy.Application.Features.Auth.Commands.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, BaseResponse<Guid>>
{
	private readonly IUserRepository _userRepository;

	public RegisterUserHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<BaseResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
	{
		if (await _userRepository.ExistsByEmailAsync(request.Email))
		{
			throw new ConflictException($"User with email {request.Email} already exists.");
		}

		string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

		var newUser = new User
		{
			Email = request.Email,
			PasswordHash = passwordHash,
			Role = request.Role,
			VerificationToken = Guid.NewGuid().ToString("N")
		};

		if (request.Role == UserRole.JobSeeker)
			newUser.JobSeekerProfile = new JobSeekerProfile { UserId = newUser.Id };
		else if (request.Role == UserRole.Employer)
			newUser.CompanyProfile = new CompanyProfile { UserId = newUser.Id };

		await _userRepository.AddAsync(newUser);

		return new BaseResponse<Guid>(newUser.Id, "User registered successfully. Please check your email to verify your account.");
	}
}