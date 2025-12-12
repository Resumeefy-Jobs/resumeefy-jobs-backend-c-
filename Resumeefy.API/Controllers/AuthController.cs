using MediatR;
using Microsoft.AspNetCore.Mvc;
using Resumeefy.Application.DTOs;
using Resumeefy.Application.Features.Auth.Commands.Login;
using Resumeefy.Application.Features.Auth.Commands.Register;
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
}