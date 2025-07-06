using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using JobPortal.Models;
using System.Collections.Generic;

namespace JobPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobPosition> JobPositions { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobPosition>()
                .Property(p => p.Salary)
                .HasColumnType("decimal(18,2)");
        }

    }
}
