using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMaze.Infrastructure.Enums;
using WebMaze.Models.Friends;
using WebMaze.Models.Friendships;
using WebMaze.Services;

namespace WebMaze.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private FriendshipService friendshipService;

        private UserService userService;

        private IMapper mapper;

        public FriendsController(FriendshipService friendshipService, UserService userService, IMapper mapper)
        {
            this.friendshipService = friendshipService;
            this.userService = userService;
            this.mapper = mapper;
        }

        public IActionResult Index([FromQuery] string searchTerm)
        {
            var user = userService.GetCurrentUser();
            var foundUsers = userService.SearchUsers(searchTerm);
            var foundUserViewModels = mapper.Map<List<FoundUserViewModel>>(foundUsers);
            foundUserViewModels.ForEach(foundUser =>
                foundUser.Friendship =
                    mapper.Map<FriendshipViewModel>(
                        friendshipService.GetFriendshipByUserLogins(user.Login, foundUser.Login)));
            
            var friendViewModels = mapper.Map<List<FriendViewModel>>(user.Friends);
            var friendRequestsViewModels = mapper.Map<List<FriendRequestViewModel>>(
                user.ReceivedFriendRequests.Where(friendship => friendship.FriendshipStatus == FriendshipStatus.Pending));

            var friendsViewModel = new FriendsViewModel
            {
                FoundUsers = foundUserViewModels,
                Friends = friendViewModels,
                FriendRequests = friendRequestsViewModels,
            };

            return View(friendsViewModel);
        }

        [HttpPost]
        public IActionResult SendFriendRequest(string userLogin)
        {
            var currentUserLogin = User.Identity.Name;
            friendshipService.CreateFriendRequest(currentUserLogin, userLogin);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }

        [HttpPost]
        public IActionResult AcceptFriendRequest(long id)
        {
            friendshipService.AcceptFriendRequest(id);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }

        [HttpPost]
        public IActionResult DeclineFriendRequest(long id)
        {
            friendshipService.DeclineFriendRequest(id);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }

        [HttpPost]
        public IActionResult CancelFriendRequest(long id)
        {
            friendshipService.CancelFriendRequest(id);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }

        [HttpPost]
        public IActionResult Unfriend(string userLogin)
        {
            var currentUserLogin = User.Identity.Name;
            friendshipService.Unfriend(currentUserLogin, userLogin);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }
    }
}
