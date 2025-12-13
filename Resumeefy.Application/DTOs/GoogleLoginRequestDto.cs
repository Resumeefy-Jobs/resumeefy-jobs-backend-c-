using Resumeefy.Core.Enums;

namespace Resumeefy.Application.DTOs
{
	public record GoogleLoginRequestDto
	{
		public string IdToken { get; set; } = string.Empty;
		public UserRole Role { get; set; } = UserRole.JobSeeker;
	}
}
