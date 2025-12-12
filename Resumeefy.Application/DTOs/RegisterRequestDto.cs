using Resumeefy.Core.Enums;

namespace Resumeefy.Application.DTOs
{
	public record RegisterRequestDto
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
		public UserRole Role { get; set; }
	}
}
