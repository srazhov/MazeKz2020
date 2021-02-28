using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.Models.Friendships;

namespace WebMaze.Models.Friends
{
    public class FoundUserViewModel
    {
        public string Login { get; set; }

        public string AvatarUrl { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public FriendshipViewModel Friendship { get; set; }
    }
}
