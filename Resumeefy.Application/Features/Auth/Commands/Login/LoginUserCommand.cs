using MediatR;
using Resumeefy.Application.DTOs;
using Resumeefy.Core.Common;

namespace Resumeefy.Application.Features.Auth.Commands.Login;

public record LoginUserCommand(string Email, string Password) : IRequest<BaseResponse<AuthResponseDto>>;