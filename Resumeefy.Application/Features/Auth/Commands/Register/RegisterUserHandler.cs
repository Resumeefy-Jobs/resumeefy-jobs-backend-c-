using Hangfire;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Resumeefy.Core.Common;
using Resumeefy.Core.Entities;
using Resumeefy.Core.Enums;
using Resumeefy.Core.Exceptions;
using Resumeefy.Core.Interfaces;
using System.Text;

namespace Resumeefy.Application.Features.Auth.Commands.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, BaseResponse<Guid>>
{
	private readonly IUserRepository _userRepository;
	private readonly IBackgroundJobClient _backgroundJobClient;
	private readonly IEmailService _emailService;
	private readonly IConfiguration _configuration;

	public RegisterUserHandler(IUserRepository userRepository, IBackgroundJobClient backgroundJobClient, IEmailService emailService, IConfiguration configuration)
	{
		_userRepository = userRepository;
		_backgroundJobClient = backgroundJobClient;
		_emailService = emailService;
		_configuration = configuration;
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
			newUser.CompanyProfile = new CompanyProfile { UserId = newUser.Id };// Ensure this is imported

		await _userRepository.AddAsync(newUser);

		string combinedString = $"{newUser.Id}:{newUser.VerificationToken}";

		byte[] bytes = Encoding.UTF8.GetBytes(combinedString);

		string code = WebEncoders.Base64UrlEncode(bytes);

		var port = _configuration["HostingPort"] ?? Environment.GetEnvironmentVariable("HostingPort");

		string verifyUrl = $"http://localhost:{port}/api/auth/verify-email?code={code}";

		string emailBody = $"<h3>Welcome to Resumeefy!</h3><p>Please click <a href='{verifyUrl}'>here</a> to verify.</p>";

		_backgroundJobClient.Enqueue<IEmailService>(email =>
			email.SendEmailAsync(newUser.Email, "Verify your account", emailBody));

		return new BaseResponse<Guid>(newUser.Id, "User registered successfully. Please check your email.");
	}
}