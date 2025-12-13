using MediatR;
using Resumeefy.Core.Common;

namespace Resumeefy.Application.Features.Auth.Commands.ForgotPassword
{
	public record ForgotPasswordCommand(string Email) : IRequest<BaseResponse<string>>;
}
