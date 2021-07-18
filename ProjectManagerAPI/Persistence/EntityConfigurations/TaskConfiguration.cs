using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class TaskConfiguration : BaseConfiguration<Task>
    {
        public override void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();

            builder.HasOne(p => p.Phase)
                .WithMany(t => t.Tasks)
                .HasForeignKey(k => k.PhaseId);

            builder.HasOne(u => u.User)
                .WithMany(t => t.Tasks)
                .HasForeignKey(k => k.UserId);

            builder.HasOne(u => u.ParentN)
                .WithMany(u => u.ChildTasks)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            base.Configure(builder);
        }
    }
}
