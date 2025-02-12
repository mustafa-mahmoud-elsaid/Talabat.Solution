using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
    public class RegisterToGetDTO
    {
        public string DisplayName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string Password { get; set; } = null!;
    }
}
