using Microsoft.EntityFrameworkCore;
using Resumeefy.Core.Entities;

namespace Resumeefy.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	// 1. Register your tables
	public DbSet<User> Users { get; set; }
	public DbSet<JobSeekerProfile> JobSeekerProfiles { get; set; }
	public DbSet<CompanyProfile> CompanyProfiles { get; set; }
	public DbSet<Skill> Skills { get; set; }
	public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }

	// 2. Configure the "Tricky" relationships (Fluent API)
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// --- User Configurations ---
		// Ensure Email is Unique in the Database
		modelBuilder.Entity<User>()
			.HasIndex(u => u.Email)
			.IsUnique();

		// 1-to-1: User -> JobSeekerProfile
		modelBuilder.Entity<User>()
			.HasOne(u => u.JobSeekerProfile)
			.WithOne(p => p.User)
			.HasForeignKey<JobSeekerProfile>(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade); // If User is deleted, profile is deleted

		// 1-to-1: User -> CompanyProfile
		modelBuilder.Entity<User>()
			.HasOne(u => u.CompanyProfile)
			.WithOne(p => p.User)
			.HasForeignKey<CompanyProfile>(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// --- Many-to-Many: JobSeeker <-> Skills ---
		// Define the Composite Primary Key (The "Junction" Table)
		modelBuilder.Entity<JobSeekerSkill>()
			.HasKey(js => new { js.JobSeekerProfileId, js.SkillId });

		// Link JobSeeker to Junction
		modelBuilder.Entity<JobSeekerSkill>()
			.HasOne(js => js.JobSeekerProfile)
			.WithMany(p => p.JobSeekerSkills)
			.HasForeignKey(js => js.JobSeekerProfileId);

		// Link Skill to Junction
		modelBuilder.Entity<JobSeekerSkill>()
			.HasOne(js => js.Skill)
			.WithMany(s => s.JobSeekerSkills)
			.HasForeignKey(js => js.SkillId);

		// --- Skill Configuration ---
		// Ensure Skill Names are unique (No duplicate "C#" entries)
		modelBuilder.Entity<Skill>()
			.HasIndex(s => s.Name)
			.IsUnique();
	}
}