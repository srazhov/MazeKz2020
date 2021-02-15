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
                var differenceInDays = difference.Days;

                switch (differenceInDays)
                {
                    case 0:
                        return TaskRelativeDate.Today;
                    case 1:
                        return TaskRelativeDate.Tomorrow;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        return TaskRelativeDate.NextSevenDays;
                    default:
                        return TaskRelativeDate.Upcoming;
                }
            }
        }
    }
}
