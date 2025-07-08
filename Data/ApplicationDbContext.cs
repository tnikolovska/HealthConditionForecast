using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthConditionForecast.Data
{
    //public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        




        public DbSet<Forecast> Forecasts { get; set; }
        public DbSet<HealthCondition> HealthConditions { get; set; }

        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<ArthritisSymtom> ArthritisSymtoms { get; set; }
        public DbSet<MigraineSympton> MigraineSymptons { get; set; }

        public DbSet<SinusSymptom> SinusSymptoms { get; set; }
        public DbSet<UserHealthCondition> UserHealthConditions { get; set; }
        public DbSet<UserSymptomSelection> UserSymptomSelections { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Forecast>().HasKey(f => f.Id);
            modelBuilder.Entity<HealthCondition>().HasKey(hc => hc.Id);
            modelBuilder.Entity<Symptom>().HasKey(s => s.Id);
            modelBuilder.Entity<UserHealthCondition>().HasKey(uhc => uhc.Id);
            modelBuilder.Entity<UserSymptomSelection>().HasKey(uss => uss.Id);
            //modelBuilder.Entity<ArthritisSymtom>().HasKey(a => a.Id);
            // modelBuilder.Entity<MigraineSympton>().HasKey(m => m.Id);
            // modelBuilder.Entity<SinusSymptom>().HasKey(sinus => sinus.Id);


            // other model configuration
        }

    }
}
