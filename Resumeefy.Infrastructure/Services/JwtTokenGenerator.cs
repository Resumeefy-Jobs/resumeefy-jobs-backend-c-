using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Resumeefy.Application.Interfaces;
using Resumeefy.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Resumeefy.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
	private readonly IConfiguration _configuration;

	public JwtTokenGenerator(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string GenerateToken(User user)
	{
		var secretKey = _configuration["JwtSettings:Secret"] ?? Environment.GetEnvironmentVariable("JwtSettings__Secret");
		var issuer = _configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JwtSettings__Issuer");
		var audience = _configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("JwtSettings__Audience");
		var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? Environment.GetEnvironmentVariable("JwtSettings__ExpiryMinutes")!);

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.Role, user.Role.ToString())
		};

		var token = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}