using Resumeefy.Core.Entities;

namespace Resumeefy.Application.Interfaces;

public interface IJwtTokenGenerator
{
	string GenerateToken(User user);
}