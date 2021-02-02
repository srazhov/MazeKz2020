using System;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.Models.Account;

namespace WebMaze.Models.Police
{
    public class PolicemanViewModel
    {
        public ProfileViewModel ProfileVM { get; set; }

        public PolicemanRank? Rank { get; set; } = null;

        public string RankString
        {
            get
            {
                return Rank switch
                {
                    PolicemanRank.NotVerified => "Гражданин",
                    PolicemanRank.Policeman => "Полицейский",
                    PolicemanRank.MorgueEmployee => "Работник морга",
                    null => "Не верифицирован",
                    _ => throw new NotImplementedException()
                };
            }
        }
    }
}
