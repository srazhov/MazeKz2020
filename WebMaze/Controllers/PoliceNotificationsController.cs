using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.DbStuff.Repository;
using WebMaze.Models.Police;

namespace WebMaze.Controllers
{
    //[Authorize(AuthenticationSchemes = Startup.PoliceAuthMethod)]
    [Route("api/[controller]")]
    [ApiController]
    public class PoliceNotificationsController : ControllerBase
    {
        private readonly PoliceNotificationsRepository pNotsRepository;
        private readonly IMapper mapper;

        public PoliceNotificationsController(PoliceNotificationsRepository pNotsRepository, IMapper mapper)
        {
            this.pNotsRepository = pNotsRepository;
            this.mapper = mapper;
        }

        [HttpGet("{userLogin}")]
        public IEnumerable<NotificationsViewModel> GetNotRead(string userLogin)
        {
            return mapper.Map<NotificationsViewModel[]>(pNotsRepository.GetAllNotReadByLogin(userLogin));
        }

        [HttpPost("WasRead")]
        public ActionResult SetWasRead([FromBody]long id)
        {
            var item = pNotsRepository.Get(id);
            if (item == null)
            {
                return BadRequest();
            }

            item.CurrentStatus = ReadStatus.WasRead;
            pNotsRepository.Save(item);
            return Ok();
        }
    }
}
