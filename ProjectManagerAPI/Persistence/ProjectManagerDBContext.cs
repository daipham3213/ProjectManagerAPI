using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Persistence.EntityConfigurations;
using System;
using ProjectManagerAPI.Core.ServiceResource;
using Task = ProjectManagerAPI.Core.Models.Task;

namespace ProjectManagerAPI.Persistence
{
    public class ProjectManagerDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public override DbSet<User> Users { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Phase> Phases { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<ServerInfo> ServerInfos { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public ProjectManagerDbContext(DbContextOptions<ProjectManagerDbContext> options)
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
            builder.ApplyConfiguration(new ServerInfoConfiguration());
            builder.ApplyConfiguration(new RequestConfiguration());
            builder.Ignore<BaseModel>();
            builder.Entity<BaseModel>().ToTable("BaseModel", t => t.ExcludeFromMigrations());
        }
    }
}
