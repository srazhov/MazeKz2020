using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze.Models.Police.Violation
{
    public class ConfirmTakeViolationViewModel
    {
        public long Id { get; set; }

        public string PolicemanLogin { get; set; }

        public bool TakeViolation { get; set; }

        public string PolicemanCommentary { get; set; }
    }
}
