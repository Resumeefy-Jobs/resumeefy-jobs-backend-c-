using Resumeefy.Core.Common;

namespace Resumeefy.Core.Entities;

public class CompanyProfile : BaseEntity
{
	public Guid UserId { get; set; }
	public virtual User User { get; set; } = null!;

	public string CompanyName { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? WebsiteUrl { get; set; }
	public string? LogoUrl { get; set; }

	public string? City { get; set; }
	public string? Country { get; set; }
	public string? Address { get; set; }
}