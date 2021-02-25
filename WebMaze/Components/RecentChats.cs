using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebMaze.Models.Friends;
using WebMaze.Services;

namespace WebMaze.Components
{
    public class RecentChats : ViewComponent
    {
        private UserService userService;

        private IMapper mapper;

        public RecentChats(UserService userService, IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }

        public IViewComponentResult Invoke()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = userService.GetCurrentUser();
                var friendViewModels = mapper.Map<List<FriendViewModel>>(user.Friends);

                return View(friendViewModels);
            }

            return View(new List<FriendViewModel>());
        }
    }
}
