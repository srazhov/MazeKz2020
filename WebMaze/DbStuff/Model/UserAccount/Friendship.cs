using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.Infrastructure.Enums;

namespace WebMaze.DbStuff.Model.UserAccount
{
    public class Friendship : BaseModel
    {
        [Required]
        public virtual DateTime RequestDate { get; set; }

        public virtual DateTime? AcceptanceDate { get; set; }

        [Required]
        public virtual FriendshipStatus FriendshipStatus { get; set; }

        [Required]
        public virtual CitizenUser Requester { get; set; }

        [Required]
        public virtual CitizenUser Requested { get; set; }
    }
}
