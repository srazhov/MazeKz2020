using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebMaze.DbStuff.Repository;

namespace WebMaze.Infrastructure
{
    public class RestrictAccessToDeadUsersHandler : AuthorizationHandler<RestrictAccessToDeadUsersRequirement>
    {
        private CitizenUserRepository citizenUserRepository;

        public RestrictAccessToDeadUsersHandler(CitizenUserRepository citizenUserRepository)
        {
            this.citizenUserRepository = citizenUserRepository;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RestrictAccessToDeadUsersRequirement requirement)
        {
            var currentUserLogin = context.User.Identity?.Name;

            if (currentUserLogin == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var deadUserLogins = citizenUserRepository.GetDeadUsers().Select(u => u.Login).ToList();

            if (!deadUserLogins.Contains(currentUserLogin))
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

    public class RestrictAccessToDeadUsersRequirement : IAuthorizationRequirement
    {
    }
}
