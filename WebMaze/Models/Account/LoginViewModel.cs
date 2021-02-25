using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebMaze.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsPersistent { get; set; }
    }
}