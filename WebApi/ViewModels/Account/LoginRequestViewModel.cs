using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels.Account
{
    public class LoginRequestViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}