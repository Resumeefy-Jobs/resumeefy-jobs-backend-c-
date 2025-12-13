using MediatR;
using Resumeefy.Core.Common;

namespace Resumeefy.Application.Features.Auth.Commands.VerifyEmail
{
	public record VerifyEmailCommand(string Code) : IRequest<BaseResponse<string>>;
}
