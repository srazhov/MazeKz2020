using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.Infrastructure.Enums;
using WebMaze.Models.Account;
using WebMaze.Models.Certificates;
using WebMaze.Models.Friends;
using WebMaze.Models.Friendships;

namespace WebMaze.Models.Users
{
    public class ProfileViewModel
    {
        public long Id { get; set; }

        public string Login { get; set; }

        public string AvatarUrl { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public bool IsMarried { get; set; }

        public FriendshipViewModel Friendship { get; set; }

        public List<FriendViewModel> Friends { get; set; }

        public List<AdressViewModel> Adresses { get; set; }

        public List<CertificateViewModel> Certificates { get; set; }
    }
}
