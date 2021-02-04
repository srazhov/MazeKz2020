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

            if (string.IsNullOrEmpty(currentUserLogin))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var currentUser = citizenUserRepository.GetUserByLogin(currentUserLogin);

            if (currentUser.IsDead)
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class RestrictAccessToDeadUsersRequirement : IAuthorizationRequirement
    {
    }
}
