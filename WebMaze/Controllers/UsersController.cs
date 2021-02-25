using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMaze.Models.Friendships;
using WebMaze.Models.Users;
using WebMaze.Services;

namespace WebMaze.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private UserService userService;

        private FriendshipService friendshipService;

        private IMapper mapper;

        public UsersController(UserService userService, FriendshipService friendshipService, IMapper mapper)
        {
            this.userService = userService;
            this.friendshipService = friendshipService;
            this.mapper = mapper;
        }

        [Route("Users/{userLogin}")]
        public IActionResult Profile(string userLogin)
        {
            var user = userService.FindByLogin(userLogin);
            var currentUserLogin = User.Identity.Name;
            var friendship = friendshipService.GetFriendshipByUserLogins(currentUserLogin, userLogin);
            var profileViewModel = mapper.Map<ProfileViewModel>(user);
            var friendshipViewModel = mapper.Map<FriendshipViewModel>(friendship);
            profileViewModel.Friendship = friendshipViewModel;

            return View(profileViewModel);
        }
    }
}
