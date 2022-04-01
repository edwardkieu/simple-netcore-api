using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}