namespace Resumeefy.Core.Entities;

public class JobSeekerSkill
{
	public Guid JobSeekerProfileId { get; set; }
	public virtual JobSeekerProfile JobSeekerProfile { get; set; } = null!;

	public Guid SkillId { get; set; }
	public virtual Skill Skill { get; set; } = null!;
}
