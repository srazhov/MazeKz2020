using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.Models.Friends;

namespace WebMaze.Models.Messenger
{
    public class ChatViewModel
    {
        public string SenderLogin { get; set; }

        public string SenderAvatarUrl { get; set; }

        public FriendViewModel Recipient { get; set; }
    }
}
