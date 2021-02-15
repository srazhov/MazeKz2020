using Microsoft.EntityFrameworkCore;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.Medicine;
using WebMaze.DbStuff.Model.Police;
using WebMaze.DbStuff.Model.UserAccount;

namespace WebMaze.DbStuff
{
    public class WebMazeContext : DbContext
    {
        public DbSet<CitizenUser> CitizenUser { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Adress> Adress { get; set; }

        public DbSet<Policeman> Policemen { get; set; }

        public DbSet<Violation> Violations { get; set; }

        public DbSet<HealthDepartment> HealthDepartment { get; set; }
        public DbSet<RecordForm> RecordForms { get; set; }

        public DbSet<Bus> Bus { get; set; }

        public DbSet<BusStop> BusStop { get; set; }

        public DbSet<BusRoute> BusRoute { get; set; }

        public DbSet<BusOrder> BusOrder { get; set; }

        public DbSet<BusWorker> BusWorker { get; set; }

        public DbSet<BusRouteTime> BusRouteTime { get; set; }

        public DbSet<UserTask> Tasks { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<MedicalInsurance> MedicalInsurances { get; set; }
        public DbSet<MedicineCertificate> MedicineCertificates { get; set; }
        public DbSet<ReceptionOfPatients> ReceptionOfPatients { get; set; }

        public WebMazeContext(DbContextOptions dbContext) : base(dbContext) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizen => citizen.Adresses)
                .WithOne(adress => adress.Owner);

            modelBuilder
                .Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.Roles)
                .WithMany(role => role.Users)
                .UsingEntity(j => j.ToTable("CitizenUserRoles"));

            modelBuilder.Entity<CitizenUser>()
                .HasOne(c => c.MedicalInsurance)
                .WithOne(m => m.Owner);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizen => citizen.RecordForms)
                .WithOne(records => records.Citizen);

            modelBuilder.Entity<CitizenUser>()
                .HasOne(p => p.MedicineCertificate)
                .WithOne(o => o.User);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(x => x.DoctorsAppointments)
                .WithOne(x => x.EnrolledCitizen);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.Tasks)
                .WithOne(userTask => userTask.Owner);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.Certificates)
                .WithOne(certificate => certificate.Owner);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.SentTransactions)
                .WithOne(transaction => transaction.Sender);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.ReceivedTransactions)
                .WithOne(transaction => transaction.Recipient);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.SentMessages)
                .WithOne(message => message.Sender)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.ReceivedMessages)
                .WithOne(message => message.Recipient)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.SentFriendRequests)
                .WithOne(friendship => friendship.Requester)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitizenUser>()
                .HasMany(citizenUser => citizenUser.ReceivedFriendRequests)
                .WithOne(friendship => friendship.Requested)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
