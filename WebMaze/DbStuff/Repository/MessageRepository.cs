using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.UserAccount;

namespace WebMaze.DbStuff.Repository
{
    public class MessageRepository : BaseRepository<Message>
    {
        public MessageRepository(WebMazeContext context) : base(context)
        {
        }


    }
}
