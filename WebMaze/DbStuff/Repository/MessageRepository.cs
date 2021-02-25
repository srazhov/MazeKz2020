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

        public IQueryable<Message> GetTwoUsersMessages(string senderLogin, string recipientLogin)
        {
            return dbSet.Where(message =>
                message.Sender.Login == senderLogin && message.Recipient.Login == recipientLogin ||
                message.Sender.Login == recipientLogin && message.Recipient.Login == senderLogin).AsQueryable();
        }

        public bool MessageWithTextExists(string messageText)
        {
            return dbSet.Any(message => message.Text == messageText);
        }
    }
}
