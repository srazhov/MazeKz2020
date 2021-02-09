using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.Models.Police
{
    public class NotificationsViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public ReadStatus CurrentStatus { get; set; }
    }
}
