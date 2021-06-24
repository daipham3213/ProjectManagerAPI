using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Persistence.EntityConfigurations;

namespace ProjectManagerAPI.Persistence
{
    public class ProjectManagerDBContext : DbContext
    {
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Phase> Phases { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Report> Reports { get; set; }

        public ProjectManagerDBContext(DbContextOptions<ProjectManagerDBContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.ApplyConfiguration(new GroupConfiguration());
            builder.ApplyConfiguration(new GroupTypeConfiguration());
            builder.ApplyConfiguration(new PhaseConfiguration());
            builder.ApplyConfiguration(new ProjectConfiguration());
            builder.ApplyConfiguration(new ReportConfiguration());
            builder.ApplyConfiguration(new TaskConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.Ignore<BaseModel>();
            builder.Entity<BaseModel>().ToTable("BaseModel", t => t.ExcludeFromMigrations());
        }
    }
}
