using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebMaze.DbStuff.Model.Medicine;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.Infrastructure.Enums;

namespace WebMaze.DbStuff.Model
{
    [Index(nameof(Login), IsUnique = true)]
    public class CitizenUser : BaseModel
    {
        [Required]
        public virtual string Login { get; set; }

        [Required]
        public virtual string Password { get; set; }

        public virtual string AvatarUrl { get; set; }

        public virtual bool IsBlocked { get; set; }

        [Column(TypeName = "money")]
        public virtual decimal Balance { get; set; }

        public virtual DateTime RegistrationDate { get; set; }

        public virtual DateTime LastLoginDate { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Email { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual Gender Gender { get; set; }

        public virtual DateTime BirthDate { get; set; }

        public virtual bool IsDead { get; set; }

        public virtual bool IsMarried { get; set; }

        public virtual bool HasChildren { get; set; }

        public virtual List<Role> Roles { get; set; } = new List<Role>();

        public virtual List<Adress> Adresses { get; set; }

        public virtual List<Certificate> Certificates { get; set; } = new List<Certificate>();

        public virtual List<UserTask> Tasks { get; set; } = new List<UserTask>();

        public virtual List<Transaction> SentTransactions { get; set; } = new List<Transaction>();

        public virtual List<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();

        public virtual List<Message> SentMessages { get; set; } = new List<Message>();

        public virtual List<Message> ReceivedMessages { get; set; } = new List<Message>();
        
        public virtual List<Friendship> SentFriendRequests { get; set; } = new List<Friendship>();

        public virtual List<Friendship> ReceivedFriendRequests { get; set; } = new List<Friendship>();

        [NotMapped]
        public virtual List<CitizenUser> Friends {
            get
            {
                var friends = SentFriendRequests
                    .Where(friendship => friendship.FriendshipStatus == FriendshipStatus.Accepted)
                    .Select(friendship => friendship.Requested).ToList();
                
                friends.AddRange(ReceivedFriendRequests
                    .Where(friendship => friendship.FriendshipStatus == FriendshipStatus.Accepted)
                    .Select(friendship => friendship.Requester));
                
                return friends;
            }
        }

        public virtual MedicalInsurance MedicalInsurance { get; set; }
        public virtual List<RecordForm> RecordForms { get; set; }
        public virtual MedicineCertificate MedicineCertificate { get; set; }
        public virtual List<ReceptionOfPatients> DoctorsAppointments { get; set; }
    }
}
