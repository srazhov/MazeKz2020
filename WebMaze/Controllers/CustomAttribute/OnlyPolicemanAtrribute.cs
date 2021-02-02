using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.DbStuff.Repository;

namespace WebMaze.Controllers.CustomAttribute
{
    public class OnlyPolicemanAttribute : ActionFilterAttribute
    {
        private PolicemanRank neededRank;
        private bool needsRankCheck;

        public OnlyPolicemanAttribute(bool needsRankCheck = true)
        {
            neededRank = PolicemanRank.Policeman;
            this.needsRankCheck = needsRankCheck;
        }

        public OnlyPolicemanAttribute(PolicemanRank policemanRank)
        {
            neededRank = policemanRank;
            needsRankCheck = true;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userName = context.HttpContext.User.Identity.Name;
            var policeRepository = context.HttpContext.RequestServices.GetService(typeof(PolicemanRepository)) as PolicemanRepository;

            if (policeRepository.IsUserPoliceman(userName, out var policeman))
            {
                if (needsRankCheck && policeman.Rank != neededRank)
                {
                    context.Result = new RedirectToActionResult("Account", "Police", null);
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("SignUp", "Police", null);
            }
        }
    }
}
