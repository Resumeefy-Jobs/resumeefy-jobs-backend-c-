using FluentValidation;

namespace Resumeefy.Application.Features.Auth.Commands.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("A valid email is required.");

		RuleFor(x => x.Password)
			.NotEmpty()
			.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
			.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches("[0-9]").WithMessage("Password must contain at least one number.");

		RuleFor(x => x.Role)
			.IsInEnum().WithMessage("Invalid role specified.");
	}
}