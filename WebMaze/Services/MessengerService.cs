using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.DbStuff.Repository;
using WebMaze.Infrastructure.Enums;

namespace WebMaze.Services
{
    public class MessengerService
    {
        private MessageRepository messageRepository;

        private UserService userService;

        private FriendshipService friendshipService;

        public MessengerService(MessageRepository messageRepository, UserService userService, FriendshipService friendshipService)
        {
            this.messageRepository = messageRepository;
            this.userService = userService;
            this.friendshipService = friendshipService;
        }

        public OperationResult SendMessage(string senderLogin, string recipientLogin, string textMessage)
        {
            var sender = userService.FindByLogin(senderLogin);
            var recipient = userService.FindByLogin(recipientLogin);

            if (sender == null || recipient == null)
            {
                var notFoundUserLogin = sender == null ? senderLogin : recipientLogin;
                return OperationResult.Failed($"User with Login = {notFoundUserLogin} not found");
            }

            var friendship = friendshipService.GetFriendshipByUserLogins(senderLogin, recipientLogin);

            if (friendship == null || friendship.FriendshipStatus != FriendshipStatus.Accepted)
            {
                return OperationResult.Failed("Sender and recipient are not friends");
            }

            var message = new Message
            {
                Date = DateTime.Now,
                Text = textMessage,
                Sender = sender,
                Recipient = recipient
            };

            messageRepository.Save(message);

            return OperationResult.Success();
        }

        public void Save(Message message)
        {
            messageRepository.Save(message);
        }

        public Message GetLastUsersMessage(string senderLogin, string recipientLogin)
        {
            var usersMessages = messageRepository.GetTwoUsersMessages(senderLogin, recipientLogin);

            return usersMessages.OrderByDescending(message => message.Date).FirstOrDefault();
        }

        public List<Message> GetLastUsersMessages(string senderLogin, string recipientLogin)
        {
            var usersMessages = messageRepository.GetTwoUsersMessages(senderLogin, recipientLogin);

            return usersMessages.OrderByDescending(message => message.Date).Take(10).Reverse().ToList();
        }
    }
}
