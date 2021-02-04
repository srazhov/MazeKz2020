using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebMaze.DbStuff.Repository;

namespace WebMaze.Infrastructure
{
    public class RestrictAccessToBlockedUsersHandler : AuthorizationHandler<RestrictAccessToBlockedUsersRequirement>
    {
        private CitizenUserRepository citizenUserRepository;

        public RestrictAccessToBlockedUsersHandler(CitizenUserRepository citizenUserRepository)
        {
            this.citizenUserRepository = citizenUserRepository;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RestrictAccessToBlockedUsersRequirement requirement)
        {
            var currentUserLogin = context.User.Identity?.Name;

            if (currentUserLogin == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var blockedUserLogins = citizenUserRepository.GetBlockedUsers().Select(u => u.Login).ToList();

            if (!blockedUserLogins.Contains(currentUserLogin))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }

    public class RestrictAccessToBlockedUsersRequirement : IAuthorizationRequirement
    {
    }
}
