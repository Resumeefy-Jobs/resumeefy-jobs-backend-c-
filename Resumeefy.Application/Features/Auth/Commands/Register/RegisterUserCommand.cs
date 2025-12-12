using MediatR;
using Resumeefy.Core.Common;
using Resumeefy.Core.Enums;

namespace Resumeefy.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(
	string Email,
	string Password,
	UserRole Role
) : IRequest<BaseResponse<Guid>>;