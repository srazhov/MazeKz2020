using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMaze.DbStuff.Model.UserAccount
{
    public class Message : BaseModel
    {
        [Required]
        public virtual DateTime Date { get; set; }

        [Required]
        public virtual string Text { get; set; }

        [Required]
        public virtual CitizenUser Sender { get; set; }

        [Required]
        public virtual CitizenUser Recipient { get; set; }
    }
}
