using Microsoft.EntityFrameworkCore;
using Resumeefy.Core.Entities;

namespace Resumeefy.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<User> Users { get; set; }
	public DbSet<JobSeekerProfile> JobSeekerProfiles { get; set; }
	public DbSet<CompanyProfile> CompanyProfiles { get; set; }
	public DbSet<Skill> Skills { get; set; }
	public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<User>()
			.HasIndex(u => u.Email)
			.IsUnique();

		modelBuilder.Entity<User>()
			.HasOne(u => u.JobSeekerProfile)
			.WithOne(p => p.User)
			.HasForeignKey<JobSeekerProfile>(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<User>()
			.HasOne(u => u.CompanyProfile)
			.WithOne(p => p.User)
			.HasForeignKey<CompanyProfile>(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<JobSeekerSkill>()
			.HasKey(js => new { js.JobSeekerProfileId, js.SkillId });

		modelBuilder.Entity<JobSeekerSkill>()
			.HasOne(js => js.JobSeekerProfile)
			.WithMany(p => p.JobSeekerSkills)
			.HasForeignKey(js => js.JobSeekerProfileId);

		modelBuilder.Entity<JobSeekerSkill>()
			.HasOne(js => js.Skill)
			.WithMany(s => s.JobSeekerSkills)
			.HasForeignKey(js => js.SkillId);

		modelBuilder.Entity<Skill>()
			.HasIndex(s => s.Name)
			.IsUnique();
	}
}