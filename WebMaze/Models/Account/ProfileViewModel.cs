using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze.Models.Account
{
    public class ProfileViewModel
    {
        public const string DefaultUserPic = "/image/Police/police_default_user_logo.png";
        private string avatar;

        public long Id { get; set; }
        public string Login { get; set; }
        public string AvatarUrl { get { return avatar ?? DefaultUserPic; } set { avatar = value; } }

        public List<AdressViewModel> Adresses { get; set; }
        
        public IFormFile Avatar { get; set; }
    }
}
