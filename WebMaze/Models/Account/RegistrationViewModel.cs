using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.Models.CustomAttribute;

namespace WebMaze.Models.Account
{
    public class RegistrationViewModel
    {
        public long Id { get; set; }

        [MinLength(3, ErrorMessage = Constants.LoginTooShortErrorMessage)]
        [UniqUserName]
        public string Login { get; set; }

        [Required(ErrorMessage = Constants.PasswordIsEmptyErrorMessage)]
        [MinLength(8, ErrorMessage = Constants.PasswordTooShortErrorMessage)]
        [AtleastOneCapital]
        [RegularExpression(@".*\d.*", ErrorMessage = Constants.PasswordMustContainDigit)]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string RepeatPassword { get; set; }
    }
}
