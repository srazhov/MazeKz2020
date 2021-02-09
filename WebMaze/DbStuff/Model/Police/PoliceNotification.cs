using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.DbStuff.Model.Police
{
    public class PoliceNotification : BaseModel
    {
        public virtual CitizenUser ToUser { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public ReadStatus CurrentStatus { get; set; }
    }
}
