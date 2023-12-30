
using Microsoft.EntityFrameworkCore;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Lockups;
using AuditingSystem.Entities.RiskAssessments;

namespace AuditingSystem.Database
{
    public class AuditingSystemDbContext : DbContext
    {
        public AuditingSystemDbContext(DbContextOptions<AuditingSystemDbContext> options)
        : base(options) { }

        public virtual DbSet<Year> Years { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<AuditUniverse> AuditUniverses { get; set; }
        public virtual DbSet<Industry> Industries { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<Entities.AuditProcess.Activity> Activities { get; set; }
        public virtual DbSet<Objective> Objectives { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<Practice> Practices { get; set; }

        public virtual DbSet<ControlEffectiveness> ControlEffectivenesses { get; set; }
        public virtual DbSet<ControlFrequency> ControlFrequencies { get; set; }
        public virtual DbSet<ControlProcess> ControlProcesses { get; set; }
        public virtual DbSet<ControlType> ControlTypes { get; set; }
        public virtual DbSet<RiskCategory> RiskCategories { get; set; }
        public virtual DbSet<RiskImpact> RiskImpacts { get; set; }
        public virtual DbSet<RiskLikehood> RiskLikehoods { get; set; }

        public virtual DbSet<RiskIdentification> RiskIdentifications { get; set; }
        public virtual DbSet<Control> Controls { get; set; }
        public virtual DbSet<RiskAssessmentList> RiskAssessmentsList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Industry>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Company>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Department>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Function>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Activity>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Objective>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Tasks>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Practice>()
                .Property(e => e.Id)
                .ValueGeneratedNever();
        }
    }
}
