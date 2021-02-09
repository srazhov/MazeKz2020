using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.DbStuff.Repository
{
    public class PoliceNotificationsRepository : BaseRepository<PoliceNotification>
    {
        public PoliceNotificationsRepository(WebMazeContext context) : base(context) { }

        public IEnumerable<PoliceNotification> GetAllByUserLogin(string userLogin)
        {
            return dbSet.Where(n => n.ToUser.Login == userLogin);
        }

        public IEnumerable<PoliceNotification> GetAllNotReadByLogin(string userLogin)
        {
            return dbSet.Where(n => n.ToUser.Login == userLogin).Where(n => n.CurrentStatus == ReadStatus.WasNotRead).OrderBy(n => n.Date);
        }

        public void SaveRange(IEnumerable<PoliceNotification> notifications)
        {
            foreach(var item in notifications)
            {
                Save(item);
            }
        }
    }
}
