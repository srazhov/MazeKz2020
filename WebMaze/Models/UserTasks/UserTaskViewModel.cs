using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.Infrastructure.Enums;

namespace WebMaze.Models.UserTasks
{
    public class UserTaskViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayName("Start date")]
        public DateTime StartDate { get; set; }

        [DisplayName("End date")]
        public DateTime EndDate { get; set; }

        public TaskStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        [DisplayName("Owner")]
        public string OwnerLogin { get; set; }

        public TaskRelativeDate RelativeDate {
            get
            {
                if (Status == TaskStatus.Complete)
                {
                    return TaskRelativeDate.Completed;
                }

                if (StartDate < DateTime.Now)
                {
                    return TaskRelativeDate.Overdue;
                }

                var difference = StartDate - DateTime.Now;
                
                if (difference > TimeSpan.FromDays(7))
                {
                    return TaskRelativeDate.Upcoming;
                }

                if (difference > TimeSpan.FromHours(48))
                {
                    return TaskRelativeDate.NextSevenDays;
                }

                if (difference > TimeSpan.FromHours(24))
                {
                    return TaskRelativeDate.Tomorrow;
                }

                return TaskRelativeDate.Today;
            }
        }
    }
}
