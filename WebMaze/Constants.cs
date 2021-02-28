using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze
{
    public static class Constants
    {
        public const string DefaultCertificateImageUrl = "/image/certificates/default_certificate.jpg";

        public const string LoginTooShortErrorMessage = "Login must be at least 3 characters";

        public const string PasswordTooShortErrorMessage = "Passwords must be at least 8 characters";

        public const string PasswordIsEmptyErrorMessage = "Enter a password";

        public const string PasswordMustContainDigit = "Passwords must have at least one digit ('0'-'9').";
    }
}
