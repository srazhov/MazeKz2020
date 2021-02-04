using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebMaze.DbStuff.Model;

namespace WebMaze.DbStuff.Repository
{
    public class CitizenUserRepository : BaseRepository<CitizenUser>
    {
        public CitizenUserRepository(WebMazeContext context) : base(context) { }

        public IQueryable<CitizenUser> GetUsersAsQueryable()
        {
            return dbSet.AsQueryable();
        }

        public IEnumerable<CitizenUser> GetUserWithHome()
        {
            return dbSet.Where(x => x.Adresses.Count() > 0);
        }

        public CitizenUser GetUserByLogin(string userLogin)
        {
            return dbSet.SingleOrDefault(user => user.Login == userLogin);
        }

        public bool UserExists(string userLogin)
        {
            return dbSet.Any(user => user.Login == userLogin);
        }

        public CitizenUser GetUserByNameAndPassword(string userName, string password)
        {
            return dbSet.SingleOrDefault(x => x.Login == userName && x.Password == password);
        }

        public CitizenUser GetUserByPassword(string password)
        {
            return dbSet.SingleOrDefault(x => x.Password == password);
        }

        public IEnumerable<CitizenUser> GetFamiliarUserNames(string userName)
        {
            userName = userName.Trim().Replace(" ", string.Empty);
            return from u in dbSet
                   where (u.FirstName + u.LastName).Contains(userName) || u.Login.Contains(userName)
                   select u;
        }

        public IQueryable<CitizenUser> GetUsersByLogins(List<string> userLogins)
        {
            return dbSet.Where(user => userLogins.Contains(user.Login));
        }

        public IQueryable<CitizenUser> GetBlockedUsers()
        {
            return dbSet.Where(user => user.IsBlocked);
        }

        public IQueryable<CitizenUser> GetDeadUsers()
        {
            return dbSet.Where(user => user.IsDead);
        }

        public IQueryable<CitizenUser> GetUsersWithCertificate(string certificateName)
        {
            return dbSet.Where(user => user.Certificates.Any(certificate => certificate.Name == certificateName));
        }
    }
}
