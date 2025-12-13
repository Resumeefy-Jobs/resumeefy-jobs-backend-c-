using MediatR;
using Resumeefy.Application.DTOs;
using Resumeefy.Core.Common;

namespace Resumeefy.Application.Features.Auth.Commands.GoogleLogin
{
	public record GoogleLoginCommand(string IdToken, Core.Enums.UserRole Role) : IRequest<BaseResponse<AuthResponseDto>>;
}
