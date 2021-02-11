using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze.Models.Friends
{
    public class FriendViewModel
    {
        [Range(0, long.MaxValue)]
        public long Id { get; set; }

        public string Login { get; set; }

        public string AvatarUrl { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
