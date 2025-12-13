using MediatR;
using Resumeefy.Core.Common;

namespace Resumeefy.Application.Features.Auth.Commands.ResetPassword
{
	public record ResetPasswordCommand(string Code, string NewPassword) : IRequest<BaseResponse<string>>;
}
