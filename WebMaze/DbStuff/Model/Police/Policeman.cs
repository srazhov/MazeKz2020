using System.Collections.Generic;
using WebMaze.DbStuff.Model.Police.Enums;

namespace WebMaze.DbStuff.Model.Police
{
    public class Policeman : BaseModel
    {
        public virtual CitizenUser User { get; set; }
        
        public PolicemanRank Rank { get; set; }

        public virtual List<Violation> Violations { get; set; }
    }
}
