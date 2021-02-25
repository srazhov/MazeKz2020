using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMaze.Infrastructure.Enums;
using WebMaze.Models.Friends;
using WebMaze.Models.Messenger;
using WebMaze.Services;

namespace WebMaze.Controllers
{
    [Authorize]
    public class MessengerController : Controller
    {
        private MessengerService messengerService;

        private UserService userService;

        private FriendshipService friendshipService;

        private IMapper mapper;

        public MessengerController(MessengerService messengerService, UserService userService, FriendshipService friendshipService, IMapper mapper)
        {
            this.messengerService = messengerService;
            this.userService = userService;
            this.friendshipService = friendshipService;
            this.mapper = mapper;
        }

        public IActionResult Index(string searchTerm)
        {
            var user = userService.GetCurrentUser();
            var userFriends = user.Friends;

            // Take user's friends whose names begin contain the search term.
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                userFriends = userFriends.Where(user =>
                    user.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                    user.LastName.ToLower().Contains(searchTerm.ToLower())).ToList();
            }

            var startedChatViewModels = new List<StartedChatViewModel>();

            foreach (var friend in userFriends)
            {
                var friendViewModel = mapper.Map<FriendViewModel>(friend);
                var lastFriendMessage = messengerService.GetLastUsersMessage(user.Login, friend.Login);
                var lastMessageViewModel = mapper.Map<MessageViewModel>(lastFriendMessage);

                if (lastMessageViewModel != null)
                {
                    lastMessageViewModel.Text = TruncateLongString(lastMessageViewModel.Text, 40);
                }

                startedChatViewModels.Add(new StartedChatViewModel()
                {
                    Friend = friendViewModel,
                    LastMessage = lastMessageViewModel
                });
            }
            
            startedChatViewModels = startedChatViewModels.OrderByDescending(startedChat => startedChat.LastMessage?.Date)
                .ToList();

            return View(startedChatViewModels);
        }

        [Route("{controller}/{recipientLogin}")]
        public IActionResult Chat(string recipientLogin)
        {
            var senderLogin = User.Identity.Name;
            var friendship = friendshipService.GetFriendshipByUserLogins(senderLogin, recipientLogin);

            if (friendship == null || friendship.FriendshipStatus != FriendshipStatus.Accepted)
            {
                return NotFound();
            }

            var friend = userService.FindByLogin(recipientLogin);
            var friendViewModel = mapper.Map<FriendViewModel>(friend);
            var sender = userService.FindByLogin(senderLogin);

            var chatViewModel = new ChatViewModel
            {
                SenderLogin = senderLogin,
                SenderAvatarUrl = sender.AvatarUrl,
                Recipient = friendViewModel
            };

            return View(chatViewModel);
        }

        private static string TruncateLongString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Substring(0, Math.Min(str.Length, maxLength)) + "...";
        }
    }
}
