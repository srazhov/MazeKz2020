using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.Infrastructure.Enums;
using WebMaze.Models.Certificates;
using WebMaze.Models.Friends;
using WebMaze.Models.Friendships;
using WebMaze.Models.UserTasks;

namespace WebMaze.Models.Account
{
    public class MyProfileViewModel
    {
        public const string DefaultUserPic = "/image/Police/police_default_user_logo.png";
        private string avatar;

        public long Id { get; set; }

        public string Login { get; set; }

        public string AvatarUrl { get { return avatar ?? DefaultUserPic; } set { avatar = value; } }

        public DateTime RegistrationDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public bool IsMarried { get; set; }

        public IFormFile Avatar { get; set; }

        public List<FriendRequestViewModel> FriendRequests { get; set; }

        public List<FriendViewModel> Friends { get; set; }

        public List<AdressViewModel> Adresses { get; set; }

        public List<CertificateViewModel> Certificates { get; set; }

        public List<UserTaskViewModel> Tasks { get; set; }


    }
}
