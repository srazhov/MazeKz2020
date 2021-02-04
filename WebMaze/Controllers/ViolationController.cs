using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.DbStuff.Repository;
using WebMaze.Models.Police.Violation;

namespace WebMaze.Controllers
{
    [Authorize(AuthenticationSchemes = Startup.PoliceAuthMethod)]
    [Route("api/[controller]")]
    [ApiController]
    public class ViolationController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ViolationRepository violationRepo;
        private readonly PolicemanRepository policeRepository;
        private readonly CitizenUserRepository userRepo;

        public ViolationController(
            IMapper mapper,
            ViolationRepository violationRepo,
            CitizenUserRepository userRepo,
            PolicemanRepository policeRepository)
        {
            this.mapper = mapper;
            this.violationRepo = violationRepo;
            this.userRepo = userRepo;
            this.policeRepository = policeRepository;
        }

        [HttpGet("{max?}")]
        public IEnumerable<ViolationItemViewModel> Get(int max = 10)
        {
            return mapper.Map<ViolationItemViewModel[]>(violationRepo.GetAll(max));
        }

        [HttpGet("SearchUsers/{word}")]
        public IEnumerable<FoundUsersViewModel> GetUsersByName(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return new FoundUsersViewModel[] { new FoundUsersViewModel { Login = string.Empty, Name = string.Empty } };
            }

            return mapper.Map<IEnumerable<FoundUsersViewModel>>(userRepo.GetFamiliarUserNames(word));
        }

        [HttpPost("Search")]
        public ActionResult<ViolationSearchItems> GetSearchItems(ViolationSearchItems searchItem)
        {
            var item = violationRepo.GetByGivenSettings(searchItem.SearchWord, searchItem.DateFrom,
                searchItem.DateTo, searchItem.Order, searchItem.ShowStatus, out int foundCount, searchItem.CurrentPage);

            searchItem.Violations = mapper.Map<ViolationItemViewModel[]>(item);
            searchItem.FoundCount = foundCount;
            searchItem.FoundOnThisPage = searchItem.Violations.Length;

            return searchItem;
        }

        [HttpPost("Declaration")]
        public ActionResult<ViolationDeclarationViewModel> AddViolationDeclaration(ViolationDeclarationViewModel item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            var violationDecl = mapper.Map<Violation>(item);
            if (!violationRepo.AddViolation(violationDecl, item.UserLogin, item.BlamedUserLogin))
            {
                return BadRequest(item);
            }

            item.RedirectLink = Url.Action("Account", "Police");
            return Ok(item);
        }

        [HttpPost("TakeViolationCase")]
        public ActionResult TakeViolationCase(ConfirmTakeViolationViewModel model)
        {
            var item = violationRepo.Get(model.Id);
            var policeman = policeRepository.GetPolicemanByLogin(model.PolicemanLogin);
            if (item == null ||
                (model.TakeViolation && item.ViewingPoliceman != null) ||
                (!model.TakeViolation && item.ViewingPoliceman != policeman))
            {
                return BadRequest();
            }

            if (item.BlamedUser.Login == model.PolicemanLogin || item.BlamingUser.Login == model.PolicemanLogin)
            {
                return BadRequest();
            }

            if (model.TakeViolation)
            {
                item.ViewingPoliceman = policeman;
                item.Status = CurrentStatus.Started;
            }
            else
            {
                item.ViewingPoliceman = null;
                item.Status = CurrentStatus.NotStarted;
            }

            violationRepo.Save(item);

            return Ok();
        }

        [HttpPost("MakeDecision")]
        public ActionResult MakeDecision(CriminalItemViewModel model)
        {
            var violation = violationRepo.Get(model.Id);
            if (violation == null || violation.ViewingPoliceman == null || violation.Status != CurrentStatus.Started)
            {
                return BadRequest();
            }

            violation.OffenseType = model.OffenseType;
            violation.Status = CurrentStatus.Accepted;
            violation.PolicemanCommentary = model.PolicemanCommentary;
            violation.ConfirmDate = DateTime.Today;

            if (model.OffenseType == TypeOfOffense.Administrative)
            {
                if (model.Penalty == null)
                {
                    return BadRequest();
                }

                violation.Penalty = model.Penalty;
                violation.TermOfPunishment = null;
            }
            else
            {
                if (model.TermOfPunishment == null)
                {
                    return BadRequest();
                }

                violation.TermOfPunishment = model.TermOfPunishment;
                violation.Penalty = null;
            }

            violationRepo.Save(violation);
            return Ok();
        }

        [HttpPost("DenyViolation")]
        public ActionResult DenyViolation(ConfirmTakeViolationViewModel model)
        {
            var violation = violationRepo.Get(model.Id);
            if(violation == null || violation.Status != CurrentStatus.Started)
            {
                return BadRequest();
            }

            violation.Status = CurrentStatus.Denied;
            violation.PolicemanCommentary = model.PolicemanCommentary;
            violation.ConfirmDate = DateTime.Today;
            violationRepo.Save(violation);

            return Ok();
        }
    }
}
