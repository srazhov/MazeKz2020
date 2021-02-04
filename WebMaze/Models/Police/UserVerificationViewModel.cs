using System;
using System.ComponentModel.DataAnnotations;
using WebMaze.DbStuff.Model;

namespace WebMaze.Models.Police
{
    public class UserVerificationViewModel
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public DateTime Birthdate { get; set; }

        public Gender Gender { get; set; } = Gender.NotChosen;

        public bool Verified { get; set; } = false;

        public bool BirthdateCapable { get => Birthdate >= new DateTime(1930, 1, 1); }

        public bool IsOldEnough { get => Birthdate <= DateTime.Today.AddYears(-18); }
    }
}
