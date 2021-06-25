using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class TaskConfiguration : BaseConfiguration
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();

            builder.HasOne(p => p.Phase)
                .WithMany(t => t.Tasks)
                .HasForeignKey(k => k.PhaseID);

            builder.HasOne(u => u.User)
                .WithMany(t => t.Tasks)
                .HasForeignKey(k => k.UserID);
        }
    }
}
