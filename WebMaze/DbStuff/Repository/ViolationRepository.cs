using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.Police.Enums;
using WebMaze.Models.Police.Violation;

namespace WebMaze.DbStuff.Repository
{
    public class ViolationRepository : BaseRepository<Violation>
    {
        public ViolationRepository(WebMazeContext context) : base(context) { }

        public Violation[] GetAll(int max)
        {
            return dbSet.Take(max).ToArray();
        }

        public bool AddViolation(Violation item, string blamingUserLogin, string blamedUserLogin)
        {
            item.BlamingUser = context.CitizenUser.SingleOrDefault(u => u.Login == blamingUserLogin);
            item.BlamedUser = context.CitizenUser.SingleOrDefault(u => u.Login == blamedUserLogin);
            if (item.BlamingUser == null || item.BlamedUser == null)
            {
                return false;
            }

            item.Status = CurrentStatus.NotStarted;
            Save(item);
            return true;
        }

        public Violation[] GetByGivenSettings(string searchword, DateTime? dateFrom, DateTime? dateTo,
            WayOfOrder orderWay, string neededStatus, out int foundTotal, int currentPage = 0, int pageMax = 50)
        {
            var result = from v in dbSet
                         where (string.IsNullOrEmpty(searchword) || (v.BlamedUser.FirstName + " " + v.BlamedUser.LastName).Contains(searchword)
                         || (v.ViewingPoliceman.User.FirstName + " " + v.ViewingPoliceman.User.LastName).Contains(searchword))
                         where (dateFrom == null || v.Date >= dateFrom) && (dateTo == null || v.Date <= dateTo)
                         select v;

            if(Enum.TryParse(neededStatus, out CurrentStatus curStatus))
            {
                result = result.Where(v => v.Status == curStatus);
            }

            result = orderWay switch
            {
                WayOfOrder.Latest => result.OrderByDescending(v => v.Date),
                WayOfOrder.Earliest => result.OrderBy(v => v.Date),
                WayOfOrder.ABCUser => result.OrderBy(v => v.BlamedUser.FirstName + " " + v.BlamedUser.LastName),
                WayOfOrder.ABCPolice => result.OrderBy(v => v.ViewingPoliceman.User.FirstName + " " + v.ViewingPoliceman.User.LastName),
                _ => throw new NotImplementedException(),
            };

            var skip = currentPage * pageMax;
            foundTotal = result.Count();
            return result.Skip(skip).Take(pageMax).ToArray();
        }
    }
}
