using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMaze.Controllers.CustomAttribute;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.DbStuff.Repository;
using WebMaze.Models.Account;
using WebMaze.Models.Police;
using WebMaze.Models.Police.Violation;

namespace WebMaze.Controllers
{
    [Authorize(AuthenticationSchemes = Startup.PoliceAuthMethod)]
    public class PoliceController : Controller
    {
        private readonly IMapper mapper;
        private readonly PolicemanRepository pmRepo;
        private readonly CitizenUserRepository cuRepo;
        private readonly ViolationRepository violationRepo;

        public PoliceController(IMapper mapper,
            PolicemanRepository pmRepo,
            CitizenUserRepository cuRepo, ViolationRepository violationRepo)
        {
            this.mapper = mapper;
            this.pmRepo = pmRepo;
            this.cuRepo = cuRepo;
            this.violationRepo = violationRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Startup.PoliceAuthMethod);
            return RedirectToAction("Index");
        }

        public IActionResult SignUp()
        {
            var user = cuRepo.GetUserByLogin(User.Identity.Name);
            var item = mapper.Map<UserVerificationViewModel>(user);

            if (pmRepo.IsUserPoliceman(user, out _))
            {
                item.Verified = true;
            }

            return View(item);
        }

        [HttpPost]
        public IActionResult SignUp(UserVerificationViewModel model)
        {
            var user = cuRepo.GetUserByLogin(User.Identity.Name);

            if (ValidateItems(user.BirthDate >= new DateTime(1930, 1, 1), model.BirthdateCapable))
            {
                user.BirthDate = model.Birthdate;
            }

            if (ValidateItems(user.Gender != Gender.NotChosen, model.Gender != Gender.NotChosen))
            {
                user.Gender = model.Gender;
            }

            if (ValidateItems(!string.IsNullOrEmpty(user.FirstName), !string.IsNullOrEmpty(model.FirstName)))
            {
                user.FirstName = model.FirstName;
            }

            if (ValidateItems(!string.IsNullOrEmpty(user.LastName), !string.IsNullOrEmpty(model.LastName)))
            {
                user.LastName = model.LastName;
            }

            cuRepo.Save(user);
            pmRepo.MakePolicemanFromUser(user);

            return RedirectToAction("VerifyUser");
        }

        [OnlyPoliceman(needsRankCheck: false)]
        public IActionResult VerifyUser()
        {
            return View(PolicemanRank.NotVerified);
        }

        [HttpPost]
        public IActionResult VerifyUser(PolicemanRank rank)
        {
            var policeman = pmRepo.GetPolicemanByLogin(User.Identity.Name);
            switch (rank)
            {
                case PolicemanRank.NotVerified:
                    return RedirectToAction("VerifyUser");
                case PolicemanRank.Policeman:
                    if (policeman.User.BirthDate <= DateTime.Today.AddYears(-18))
                    {
                        policeman.Rank = PolicemanRank.Policeman;
                        pmRepo.Save(policeman);
                    }
                    else
                    {
                        return View(PolicemanRank.Policeman);
                    }

                    break;
                case PolicemanRank.MorgueEmployee:
                    // Аутентификация в аккаунт морга. Если данного пользователя там нет, 
                    // то предложить пользователю перейти в сайт Морга, и зарегистрироваться там
                    return View(PolicemanRank.MorgueEmployee);
                default:
                    throw new NotImplementedException();
            }

            return RedirectToAction("Account");
        }

        public IActionResult Account()
        {
            var userItem = cuRepo.GetUserByLogin(User.Identity.Name);
            if (userItem == null)
            {
                throw new NotImplementedException();
            }

            if (!pmRepo.IsUserPoliceman(userItem, out Policeman policeItem))
            {
                return View(new PolicemanViewModel { ProfileVM = mapper.Map<ProfileViewModel>(userItem) });
            }

            var result = mapper.Map<PolicemanViewModel>(policeItem);
            return View(result);
        }

        [Route("[controller]/[action]/{id?}")]
        [OnlyPoliceman]
        public IActionResult Criminal(long? id)
        {
            if (id == -1)
            {
                return View(true);
            }

            var violation = violationRepo.Get(id.GetValueOrDefault());
            if (violation == null)
            {
                return View(false);
            }

            var item = mapper.Map<CriminalItemViewModel>(violation);
            item.PolicemanCanTakeThisViolation =
                violation.BlamingUser?.Login != User.Identity.Name
                && violation.BlamedUser?.Login != User.Identity.Name;
            
            return View("CriminalItem", item);
        }

        [OnlyPoliceman(needsRankCheck: false)]
        public IActionResult AddViolation()
        {
            var user = cuRepo.GetUserByLogin(User.Identity.Name);
            var result = new ViolationRegistrationViewModel()
            {
                UserName = $"{user.FirstName} {user.LastName}",
                UserLogin = user.Login,
                Date = DateTime.Today
            };

            return View(result);
        }

        // Anonymous methods ------------------------------------------------

        [AllowAnonymous]
        public IActionResult Index()
        {
            var card = new CardViewModel
            {
                SubTitle = "Внутри системы",
                Title = "Получить сертификат полицейского",
                Description = "У вас есть страсть бороться с преступностью? Тогда вам к нам."
                    + "Отправьте заявку на получение сертификата не выходя из дома!",
                Link = "#",
                LinkText = "Получить сейчас"
            };

            var result = new MainIndexInfoViewModel()
            {
                Cards = new List<CardViewModel> { card }
            };

            return View(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            var userItem = cuRepo.GetUserByLogin(user.Login);
            if (userItem == null)
            {
                user.Login = string.Empty;
                ModelState.AddModelError("Login", "Данный логин не существует");
            }
            else if (userItem.Password != user.Password)
            {
                ModelState.AddModelError("Password", "Неправильный пароль");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            await AuthorizeUser(userItem.Id, userItem.Login);

            if (string.IsNullOrEmpty(user.ReturnUrl))
            {
                return RedirectToAction("Index");
            }

            return Redirect(user.ReturnUrl);
        }


        // Private methods ----------------------------------------------------

        private async Task AuthorizeUser(long userId, string login)
        {
            var claims = new List<Claim>()
            {
                new Claim("Id", userId.ToString()),
                new Claim(ClaimTypes.AuthenticationMethod, Startup.PoliceAuthMethod),
                new Claim(ClaimTypes.Name, login)
            };

            var id = new ClaimsIdentity(claims, Startup.PoliceAuthMethod);
            await HttpContext.SignInAsync(Startup.PoliceAuthMethod, new ClaimsPrincipal(id));
        }
        private bool ValidateItems(bool userHasValue, bool modelHasValue)
        {
            if (userHasValue || !modelHasValue)
            {
                RedirectToAction("SignUp");
                return false;
            }

            return true;
        }
    }
}
