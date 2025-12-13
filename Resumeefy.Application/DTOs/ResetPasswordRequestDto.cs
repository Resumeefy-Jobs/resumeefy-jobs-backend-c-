namespace Resumeefy.Application.DTOs
{
	public record ResetPasswordRequestDto
	{
		public string Code { get; set; } = string.Empty; // The long code from the URL
		public string NewPassword { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
