using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.UserAccount;

namespace WebMaze.DbStuff.Repository
{
    public class UserTaskRepository : BaseRepository<UserTask>
    {
        public UserTaskRepository(WebMazeContext context) : base(context)
        {
        }

        public IQueryable<UserTask> GetTasksByUserLogin(string userLogin)
        {
            return dbSet.Where(userTask => userTask.Owner.Login == userLogin);
        }

        public bool TaskExists(long taskId)
        {
            return dbSet.Any(task => task.Id == taskId);
        }

        public bool TaskWithNameExists(string taskName)
        {
            return dbSet.Any(task => task.Name == taskName);
        }
    }
}
