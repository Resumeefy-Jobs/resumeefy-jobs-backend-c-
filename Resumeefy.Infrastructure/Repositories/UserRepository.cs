using Microsoft.EntityFrameworkCore;
using Resumeefy.Core.Entities;
using Resumeefy.Core.Interfaces;
using Resumeefy.Infrastructure.Data;

namespace Resumeefy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _context;

	public UserRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		return await _context.Users
			.Include(u => u.JobSeekerProfile)
			.Include(u => u.CompanyProfile)
			.FirstOrDefaultAsync(u => u.Email == email);
	}

	public async Task<bool> ExistsByEmailAsync(string email)
	{
		return await _context.Users.AnyAsync(u => u.Email == email);
	}

	public async Task<User?> GetByIdAsync(Guid id)
	{
		return await _context.Users.FindAsync(id);
	}

	public async Task AddAsync(User user)
	{
		await _context.Users.AddAsync(user);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateAsync(User user)
	{
		_context.Users.Update(user);
		await _context.SaveChangesAsync();
	}
}