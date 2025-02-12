using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
    public class LoginToGetDTO
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
