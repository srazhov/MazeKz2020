using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze.Models.Friends
{
    public class FriendsViewModel
    {
        public List<FriendRequestViewModel> FriendRequests { get; set; }

        public List<FriendViewModel> Friends { get; set; }

        public List<FoundUserViewModel> FoundUsers { get; set; }
    }
}
