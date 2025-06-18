using hope4life.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace hope4life.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Models.Entities.Donor> Donors { get; set; }
        public DbSet<Models.Entities.Patient> Patients { get; set; }
        public DbSet<Models.Entities.EmergencyRequest> EmergencyRequests { get; set; }

        public DbSet<DonorAssignment> DonorAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // turn off cascade everywhere for safety
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                                                .SelectMany(t => t.GetForeignKeys()))
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            modelBuilder.Entity<DonorAssignment>()
                .ToTable("DonorAssignments", tb => tb.HasTrigger("trg_AssignmentInsertNotification"));
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StatusMaster> StatusMasters { get; set; }





    }

}

