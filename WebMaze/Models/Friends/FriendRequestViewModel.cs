using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze.Models.Friends
{
    public class FriendRequestViewModel
    {
        [Range(0, long.MaxValue)]
        public long Id { get; set; }

        public DateTime RequestDate { get; set; }

        public FriendViewModel Requester { get; set; }
    }
}
