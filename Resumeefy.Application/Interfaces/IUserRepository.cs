using Resumeefy.Core.Entities;

namespace Resumeefy.Core.Interfaces;

public interface IUserRepository
{
	Task<User?> GetByEmailAsync(string email);
	Task<bool> ExistsByEmailAsync(string email);
	Task<User?> GetByIdAsync(Guid id);

	Task AddAsync(User user);
	Task UpdateAsync(User user);

}