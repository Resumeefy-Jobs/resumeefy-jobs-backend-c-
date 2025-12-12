using Resumeefy.Core.Common;
using Resumeefy.Core.Enums;

namespace Resumeefy.Core.Entities;

public class User : BaseEntity
{
	public string Email { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public UserRole Role { get; set; }

	public bool IsActive { get; set; } = true;
	public bool IsEmailVerified { get; set; } = false;


	public string? VerificationToken { get; set; }
	public string? PasswordResetToken { get; set; }
	public DateTime? TokenExpiresAt { get; set; }


	public virtual JobSeekerProfile? JobSeekerProfile { get; set; }
	public virtual CompanyProfile? CompanyProfile { get; set; }


	public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}