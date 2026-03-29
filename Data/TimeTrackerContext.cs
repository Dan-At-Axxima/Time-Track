using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTrackerRepo.Models;

namespace TimeTrackerRepo.Data
{
    public class TimeTrackerContext : DbContext
    {
        public TimeTrackerContext(DbContextOptions<TimeTrackerContext> options)
     : base(options) {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<V_AssistedEmployee> AssistedEmployee { get; set; }
        public DbSet<Assignments> Assignments { get; internal set; }
        public DbSet<Transactions> Transactions { get; internal set; }
        public DbSet<ReportFreezeDate> ReportFreezeDate { get; set; }
        public DbSet<Activities>  Activity { get; internal set; }
        public DbSet<Logs> Logs { get; internal set; }
        public DbSet<EmployeeAndRates> EmployeeAndRates { get; internal set; }  
        public DbSet<Rates> Rates { get; internal set; }
        public DbSet<EmployeeAndRatesWithHours> EmployeeAndRatesWithHours { get; internal set; }

        public DbSet<NewSettings> NewSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewSettings>(entity =>
            entity.HasKey(p => new { p.EmployeeNumber}));
            modelBuilder.Entity<Assignments>(entity =>
            entity.HasKey(p => new { p.EmployeeNumber, p.Client, p.Project, p.Activity }));
            modelBuilder.Entity<Transactions>(entity =>
            entity.HasKey(p => new { p.EmployeeNumber, p.Client,p.Project, p.Activity, p.Date}));
            modelBuilder.Entity<Activities>(entity =>
            entity.HasKey(p => new { p.Client, p.Project,  p.Activity }));
            modelBuilder.Entity<ReportFreezeDate>(entity => entity.HasKey(p => new { p.FrozenDate }));
        }
    }
}
