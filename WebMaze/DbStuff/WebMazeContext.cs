﻿using Microsoft.EntityFrameworkCore;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.Medicine;
using WebMaze.DbStuff.Model.Police;

namespace WebMaze.DbStuff
{
    public class WebMazeContext : DbContext
    {
        public DbSet<CitizenUser> CitizenUser { get; set; }

        public DbSet<Adress> Adress { get; set; }

        public DbSet<Policeman> Policemen { get; set; }

        public DbSet<Violation> Violations { get; set; }
        
        public DbSet<ViolationType> TypesOfViolation { get; set; }

        public DbSet<PoliceCertificate> PoliceCertificates { get; set; }

        public DbSet<HealthDepartment> HealthDepartment { get; set; }
        public DbSet<RecordForm> RecordForms { get; set; }

        public DbSet<Bus> Bus { get; set; }

        public DbSet<BusStop> BusStop { get; set; }

        public DbSet<BusRoute> BusRoute { get; set; }

        public DbSet<BusOrder> BusOrder { get; set; }

        public DbSet<BusWorker> BusWorker { get; set; }

        public DbSet<BusRouteTime> BusRouteTime { get; set; }

        public DbSet<UserTask> UserTasks { get; set; }

        public DbSet<MedicalInsurance> MedicalInsurances { get; set; }

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

            modelBuilder.Entity<CitizenUser>()
                .HasOne(c => c.MedicalInsurance)
                .WithOne(m => m.Owner);


            base.OnModelCreating(modelBuilder);
        }
    }
}
