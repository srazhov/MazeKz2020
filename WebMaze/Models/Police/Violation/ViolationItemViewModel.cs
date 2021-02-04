using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.Models.Police.Violation
{
    public class ViolationItemViewModel
    {
        private string policemanname;

        public long Id { get; set; }

        public string BlamedUserName { get; set; }

        public string PolicemanName { get { return policemanname ?? "---"; } set { policemanname = value; } }

        public DateTime Date { get; set; }

        public CurrentStatus Status { get; set; }
    }
}
