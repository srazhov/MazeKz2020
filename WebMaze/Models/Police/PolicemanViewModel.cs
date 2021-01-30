using System;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.Models.Account;

namespace WebMaze.Models.Police
{
    public class PolicemanViewModel
    {
        public ProfileViewModel ProfileVM { get; set; }

        public PolicemanRank Rank { get; set; } = PolicemanRank.NotVerified;

        public bool IsPoliceman { get => Rank != PolicemanRank.NotVerified; }

        public string RankString
        {
            get
            {
                return Rank switch
                {
                    PolicemanRank.NotVerified => "Не верифицирован",
                    PolicemanRank.Policeman => "Полицейский",
                    PolicemanRank.MorgueEmployee => "Работник морга",
                    _ => throw new NotImplementedException()
                };
            }
        }
    }
}
