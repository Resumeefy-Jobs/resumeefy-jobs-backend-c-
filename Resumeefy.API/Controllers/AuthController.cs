using MediatR;
using Microsoft.AspNetCore.Mvc;
using Resumeefy.Application.DTOs;
using Resumeefy.Application.DTOs.Auth;
using Resumeefy.Application.Features.Auth.Commands.ForgotPassword;
using Resumeefy.Application.Features.Auth.Commands.GoogleLogin;
using Resumeefy.Application.Features.Auth.Commands.Login;
using Resumeefy.Application.Features.Auth.Commands.Register;
using Resumeefy.Application.Features.Auth.Commands.ResetPassword;
using Resumeefy.Application.Features.Auth.Commands.VerifyEmail;
using Resumeefy.Core.Common;

namespace Resumeefy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;

	public AuthController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("register")]
	public async Task<ActionResult<BaseResponse<Guid>>> Register([FromBody] RegisterRequestDto request)
	{
		var command = new RegisterUserCommand(request.Email, request.Password, request.Role);

		var result = await _mediator.Send(command);

		return Ok(result);
	}

	[HttpPost("login")]
	public async Task<ActionResult<BaseResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
	{
		var command = new LoginUserCommand(request.Email, request.Password);
		var result = await _mediator.Send(command);
		return Ok(result);
	}

	[HttpGet("verify-email")]
	public async Task<ActionResult<BaseResponse<string>>> VerifyEmail([FromQuery] string code)
	{
		var command = new VerifyEmailCommand(code);
		var result = await _mediator.Send(command);
		return Ok(result);
	}

	[HttpPost("forgot-password")]
	public async Task<ActionResult<BaseResponse<string>>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
	{
		var command = new ForgotPasswordCommand(request.Email);
		var result = await _mediator.Send(command);
		return Ok(result);
	}

	[HttpPost("reset-password")]
	public async Task<ActionResult<BaseResponse<string>>> ResetPassword([FromBody] ResetPasswordRequestDto request)
	{
		var command = new ResetPasswordCommand(request.Code, request.NewPassword);
		var result = await _mediator.Send(command);
		return Ok(result);
	}

	[HttpPost("google-login")]
	public async Task<ActionResult<BaseResponse<AuthResponseDto>>> GoogleLogin([FromBody] GoogleLoginRequestDto request)
	{
		var command = new GoogleLoginCommand(request.IdToken, request.Role);
		var result = await _mediator.Send(command);
		return Ok(result);
	}
}