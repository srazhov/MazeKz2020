using System;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.Models.Account;

namespace WebMaze.Models.Police
{
    public class PolicemanViewModel
    {
        public const string DefaultUserPic = "/image/Police/police_default_user_logo.png";
        private string avatar;

        public string Login { get; set; }

        public string AvatarUrl { get { return avatar ?? DefaultUserPic; } set { avatar = value; } }

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
