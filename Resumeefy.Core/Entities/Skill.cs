using Resumeefy.Core.Common;

namespace Resumeefy.Core.Entities;

public class Skill : BaseEntity
{
	public string Name { get; set; } = string.Empty;

	public virtual ICollection<JobSeekerSkill> JobSeekerSkills { get; set; } = new List<JobSeekerSkill>();
}