using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Resumeefy.Application.Interfaces;
using Resumeefy.Core.Entities;
using Resumeefy.Core.Exceptions;

namespace Resumeefy.Infrastructure.Services
{
	public class GoogleAuthService : IGoogleAuthService
	{
		private readonly IConfiguration _configuration;

		public GoogleAuthService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<GoogleUserResult> ValidateAsync(string idToken)
		{
			try
			{
				var googleClientid = _configuration["Google:ClientId"] ?? Environment.GetEnvironmentVariable("Google__ClientId");
				var settings = new GoogleJsonWebSignature.ValidationSettings()
				{
					Audience = new List<string> { googleClientid! }
				};

				var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

				return new GoogleUserResult
				{
					Email = payload.Email,
					FirstName = payload.GivenName,
					LastName = payload.FamilyName,
					PictureUrl = payload.Picture
				};
			}
			catch (InvalidJwtException)
			{
				throw new ApiException("Invalid Google Token.");
			}
		}
	}
}
