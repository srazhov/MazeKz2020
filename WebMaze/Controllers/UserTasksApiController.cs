using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.Models.UserTasks;
using WebMaze.Services;
using WebMaze.Infrastructure.Enums;

namespace WebMaze.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class UserTasksApiController : ControllerBase
    {
        private TaskService taskService;

        private UserService userService;

        private IMapper mapper;

        public UserTasksApiController(TaskService taskService, UserService userService, IMapper mapper)
        {
            this.taskService = taskService;
            this.userService = userService;
            this.mapper = mapper;
        }

        // GET: api/tasks
        [HttpGet]
        public ActionResult<IEnumerable<UserTaskViewModel>> GetTasks([FromQuery] string relativeDateString = "Upcoming")
        {
            var parsed = Enum.TryParse(relativeDateString, out TaskRelativeDate relativeDate);

            if (!parsed)
            {
                return BadRequest("relativeDateString is invalid");
            }

            var currentUserLogin = User.Identity.Name;
            var userTasks = relativeDate switch
            {
                TaskRelativeDate.Today => taskService.GetUserTasksPlannedForToday(currentUserLogin),
                TaskRelativeDate.Tomorrow => taskService.GetUserTasksPlannedForTomorrow(currentUserLogin),
                TaskRelativeDate.NextSevenDays => taskService.GetUserTasksPlannedForNextDays(currentUserLogin, 7),
                TaskRelativeDate.Upcoming => taskService.GetUserPlannedTasks(currentUserLogin),
                TaskRelativeDate.Completed => taskService.GetUserCompletedTasks(currentUserLogin),
                _ => throw new ArgumentOutOfRangeException()
            };

            var overdueTasks = taskService.GetUserOverdueTasks(currentUserLogin);
            var combinedTasks = overdueTasks.Concat(userTasks);
            var taskViewModels = mapper.Map<List<UserTaskViewModel>>(combinedTasks);

            return taskViewModels;
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public ActionResult<UserTaskViewModel> GetTask(long id)
        {
            var task = taskService.GetTaskById(id);

            if (task == null)
            {
                return NotFound($"UserTask with ID = {id} not found");
            }

            var taskViewModel = mapper.Map<UserTaskViewModel>(task);

            return taskViewModel;
        }

        // POST: api/tasks
        [HttpPost]
        public ActionResult<UserTaskViewModel> PostTask(UserTaskViewModel taskViewModel)
        {
            // Exclude property from binding.
            taskViewModel.Id = 0;

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(modelStateEntry => modelStateEntry.Errors.Select(b => b.ErrorMessage)).ToList();
                
                return BadRequest(errorMessages);
            }

            var user = userService.GetUserByLogin(taskViewModel.OwnerLogin);

            if (user == null)
            {
                return BadRequest(new List<string>() { $"User with Login = {taskViewModel.OwnerLogin} not found" });
            }

            var task = mapper.Map<UserTask>(taskViewModel);
            task.Owner = user;
            taskService.Save(task);

            taskViewModel = mapper.Map<UserTaskViewModel>(task);

            return CreatedAtAction("GetTask", new { id = taskViewModel.Id }, taskViewModel);
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public IActionResult PutTask(long id, UserTaskViewModel taskViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(modelStateEntry => modelStateEntry.Errors.Select(b => b.ErrorMessage)).ToList();
                return BadRequest(errorMessages);
            }

            if (id != taskViewModel.Id)
            {
                return BadRequest(new List<string>() { "Task ID mismatch" });
            }

            var task = taskService.GetTaskById(id);

            if (task == null)
            {
                return NotFound(new List<string>() { $"Task with ID = {id} not found" });
            }

            var user = userService.GetUserByLogin(taskViewModel.OwnerLogin);

            if (user == null)
            {
                return NotFound(new List<string>() { $"User with Login = {taskViewModel.OwnerLogin} not found" });
            }

            mapper.Map(taskViewModel, task);
            task.Owner = user;
            taskService.Save(task);

            return NoContent();
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(long id)
        {
            if (taskService.Exists(id))
            {
                return NotFound(new List<string>() { $"UserTask with ID = {id} not found" });
            }

            taskService.Delete(id);

            return NoContent();
        }
    }
}
