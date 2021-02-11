using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.UserAccount;

namespace WebMaze.DbStuff.Repository
{
    public class FriendshipRepository : BaseRepository<Friendship>
    {
        public FriendshipRepository(WebMazeContext context) : base(context)
        {
        }

        public bool FriendshipExists(long friendshipId)
        {
            return dbSet.Any(friendship => friendship.Id == friendshipId);
        }

        public bool FriendshipBetweenUsersExists(string userLogin1, string userLogin2)
        {
            return dbSet.Any(friendship => friendship.Requester.Login == userLogin1 && friendship.Requested.Login == userLogin2
                                           || friendship.Requester.Login == userLogin2 && friendship.Requested.Login == userLogin1);
        }

        public Friendship GetFriendshipByUserLogins(string userLogin1, string userLogin2)
        {
            return dbSet.SingleOrDefault(friendship => friendship.Requester.Login == userLogin1 && friendship.Requested.Login == userLogin2
                                                       || friendship.Requester.Login == userLogin2 && friendship.Requested.Login == userLogin1);
        }
    }
}
