using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMaze.DbStuff;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Repository;
using WebMaze.Models.Account;
using WebMaze.Services;

namespace WebMaze.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private CitizenUserRepository citizenUserRepository;
        private AdressRepository adressRepository;
        private IWebHostEnvironment hostEnvironment;
        private IMapper mapper;
        private UserService userService;
        private FriendshipService friendshipService;

        public AccountController(CitizenUserRepository citizenUserRepository, IMapper mapper,
            IWebHostEnvironment hostEnvironment, AdressRepository adressRepository, UserService userService,
            FriendshipService friendshipService)
        {
            this.citizenUserRepository = citizenUserRepository;
            this.mapper = mapper;
            this.hostEnvironment = hostEnvironment;
            this.adressRepository = adressRepository;
            this.userService = userService;
            this.friendshipService = friendshipService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var viewModel = new LoginViewModel();

            viewModel.ReturnUrl = Request.Query["ReturnUrl"];

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            /* Authentication written by Pavel:
            var user = citizenUserRepository
                .GetUserByNameAndPassword(loginViewModel.Login, loginViewModel.Password);
            if (user == null)
            {
                return View(loginViewModel);
            }

            
            //Строки в документе
            var recordId = new Claim("Id", user.Id.ToString());
            var recordName = new Claim(ClaimTypes.Name, user.Login);
            var recordAuthMethod = new Claim(ClaimTypes.AuthenticationMethod, Startup.AuthMethod);

            //Страница в документе
            var page = new List<Claim>() { recordId, recordName, recordAuthMethod };

            //Документ
            var claimsIdentity = new ClaimsIdentity(page, Startup.AuthMethod);

            //Пользователь с точки зрения .net
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);
            */

            if (ModelState.IsValid)
            {
                var operationResult = await userService.SignInAsync(loginViewModel.Login, loginViewModel.Password, loginViewModel.IsPersistent);
                if (operationResult.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return Redirect(loginViewModel.ReturnUrl);
                }

                foreach (var error in operationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(loginViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registration()
        {
            var viewModel = new RegistrationViewModel()
            {
                Login = "Test",
                Password = "Test"
            };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registration(RegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = mapper.Map<CitizenUser>(viewModel);
            userService.Save(user);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult MyProfile()
        {
            var user = userService.GetCurrentUser();
            var viewModel = mapper.Map<MyProfileViewModel>(user);
            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public IActionResult MyProfile(MyProfileViewModel profileViewModel)
        {
            var citizen = mapper.Map<CitizenUser>(profileViewModel);
            citizenUserRepository.Save(citizen);
            return View(profileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            var user = userService.GetCurrentUser();
            var fileName = user.Login;
            var wwwrootPath = hostEnvironment.WebRootPath;
            var path = @$"{wwwrootPath}\image\avatar\{fileName}.jpg";
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await avatar.CopyToAsync(fileStream);
            }

            user.AvatarUrl = $"/image/avatar/{fileName}.jpg";
            userService.Save(user);

            return RedirectToAction("MyProfile");
        }
    
        [HttpGet]
        public IActionResult AddAdress(long userId)
        {
            var viewModel = new AdressViewModel()
            {
                OwnerId = userId
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddAdress(AdressViewModel adressViewModel)
        {
            var adress = mapper.Map<Adress>(adressViewModel);
            var user = citizenUserRepository.Get(adressViewModel.OwnerId);

            adress.Owner = user;

            adressRepository.Save(adress);

            return RedirectToAction("MyProfile", new { id = adressViewModel.OwnerId });
        }
    }
}
