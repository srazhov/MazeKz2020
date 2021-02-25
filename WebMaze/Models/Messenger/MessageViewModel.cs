using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebMaze.DbStuff.Model;

namespace WebMaze.Models.Messenger
{
    public class MessageViewModel
    {
        [Required]
        public string Date { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string SenderLogin { get; set; }

        [Required]
        public string RecipientLogin { get; set; }
    }
}
