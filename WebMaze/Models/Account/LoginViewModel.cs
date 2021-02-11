using System.ComponentModel;

namespace WebMaze.Models.Account
{
    public class LoginViewModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsPersistent { get; set; }
    }
}