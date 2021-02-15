using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.DbStuff.Repository;
using WebMaze.Infrastructure.Enums;
using WebMaze.Models.UserTasks;
using WebMaze.Services;

namespace WebMaze.Controllers
{
    [Authorize]
    public class UserTasksController : Controller
    {
        private TaskService taskService;

        private UserService userService;

        private IMapper mapper;

        public UserTasksController(TaskService taskService, UserService userService, IMapper mapper)
        {
            this.taskService = taskService;
            this.userService = userService;
            this.mapper = mapper;
        }

        [Route("Tasks/{relativeDateString}")]
        public IActionResult Tasks(string relativeDateString)
        {
            var parsed = Enum.TryParse(relativeDateString, out TaskRelativeDate relativeDate);

            if (!parsed)
            {
                return NotFound();
            }

            var currentUserLogin = User.Identity.Name;
            var userTasks = relativeDate switch
            {
                TaskRelativeDate.Today => taskService.GetUserTasksPlannedForToday(currentUserLogin),
                TaskRelativeDate.Tomorrow => taskService.GetUserTasksPlannedForTomorrow(currentUserLogin),
                TaskRelativeDate.NextSevenDays => taskService.GetUserTasksPlannedForNextDays(currentUserLogin, 7),
                TaskRelativeDate.Upcoming => taskService.GetUserPlannedTasks(currentUserLogin),
                TaskRelativeDate.Completed => taskService.GetUserCompletedTasks(currentUserLogin),
                TaskRelativeDate.Overdue => taskService.GetUserCompletedTasks(currentUserLogin),
                _ => throw new ArgumentOutOfRangeException()
            };

            var overdueTasks = taskService.GetUserOverdueTasks(currentUserLogin);
            var combinedTasks = overdueTasks.Concat(userTasks);
            var taskViewModels = mapper.Map<List<UserTaskViewModel>>(combinedTasks);
            ViewData["Title"] = relativeDate == TaskRelativeDate.NextSevenDays ? "Tasks for next seven days"
                : relativeDate.ToString() + " tasks";

            return View(taskViewModels);
        }

        public IActionResult Create()
        {
            return View(nameof(Edit));
        }

        public IActionResult Edit(long id)
        {
            var userTask = taskService.GetTaskById(id);
            var userTaskViewModel = mapper.Map<UserTaskViewModel>(userTask);

            return View(userTaskViewModel);
        }

        [HttpPost]
        public IActionResult Edit(UserTaskViewModel userTaskViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userTaskViewModel);
            }

            var userTask = mapper.Map<UserTask>(userTaskViewModel);
            var user = userService.FindByLogin(userTaskViewModel.OwnerLogin);
            userTask.Owner = user;
            taskService.Save(userTask);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            taskService.Delete(id);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }

        [HttpPost]
        public IActionResult Complete(long id)
        {
            var userTask = taskService.GetTaskById(id);
            userTask.Status = TaskStatus.Complete;
            taskService.Save(userTask);
            var urlReferrer = Request.Headers["Referer"].ToString();

            return Redirect(urlReferrer);
        }
    }
}
