// DTOs/PasswordResetDto.cs
namespace webapi.DTOs
{
    public class PasswordResetDto
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}