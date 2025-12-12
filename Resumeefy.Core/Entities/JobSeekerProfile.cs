using Resumeefy.Core.Common;
using Resumeefy.Core.Enums;

namespace Resumeefy.Core.Entities;

public class JobSeekerProfile : BaseEntity
{
	public Guid UserId { get; set; }
	public virtual User User { get; set; } = null!;

	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string? Bio { get; set; }
	public string? ResumeUrl { get; set; }
	public string? PortfolioUrl { get; set; }
	public ExperienceLevel ExperienceLevel { get; set; }

	public virtual ICollection<JobSeekerSkill> JobSeekerSkills { get; set; } = new List<JobSeekerSkill>();
}