using Resumeefy.Core.Entities;

namespace Resumeefy.Application.Interfaces
{
	public interface IGoogleAuthService
	{
		Task<GoogleUserResult> ValidateAsync(string idToken);
	}
}
