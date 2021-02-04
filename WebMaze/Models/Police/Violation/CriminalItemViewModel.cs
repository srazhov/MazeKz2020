using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.Models.Police.Violation
{
    public class CriminalItemViewModel
    {
        private string avatarUrl;

        public long Id { get; set; }

        public bool PolicemanCanTakeThisViolation { get; set; }

        public string BlamingUserName { get; set; }

        public string BlamedUserName { get; set; }

        public string BlamedUserAvatar
        {
            get { return avatarUrl ?? "/image/Police/user-not-found.jpg"; }
            set { avatarUrl = value; }
        }

        public string ViewingPolicemanName { get; set; }

        public string PolicemanLogin { get; set; }

        public DateTime Date { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public string Explanation { get; set; }

        public string PolicemanCommentary { get; set; }

        public TypeOfOffense OffenseType { get; set; }

        public CurrentStatus Status { get; set; }

        public DateTime? TermOfPunishment { get; set; }

        public decimal? Penalty { get; set; }
    }
}
