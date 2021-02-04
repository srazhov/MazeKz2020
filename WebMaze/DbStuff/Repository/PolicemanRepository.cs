using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.DbStuff.Repository
{
    public class PolicemanRepository : BaseRepository<Policeman>
    {
        public PolicemanRepository(WebMazeContext context) : base(context) { }

        public Policeman GetPolicemanByLogin(string userLogin)
        {
            return dbSet.SingleOrDefault(u => u.User.Login == userLogin);
        }

        public bool IsUserPoliceman(CitizenUser user, out Policeman output)
        {
            output = dbSet.Where(p => p.User.Login == user.Login).SingleOrDefault();
            return output != null;
        }

        public bool IsUserPoliceman(string userLogin, out Policeman output)
        {
            output = dbSet.Where(p => p.User.Login == userLogin).SingleOrDefault();
            return output != null;
        }

        public void MakePolicemanFromUser(CitizenUser user)
        {
            var policeman = new Policeman() { User = user, Rank = PolicemanRank.NotVerified };
            Save(policeman);
        }
    }
}
