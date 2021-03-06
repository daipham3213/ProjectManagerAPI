using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class ReportConfiguration : BaseConfiguration<Report>
    {
        public override void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();

            builder.HasOne(p => p.Project).WithMany(r => r.Reports)
                .HasForeignKey(k => k.ProjectId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Group).WithMany(r => r.Reports)
                .HasForeignKey(k => k.GroupId);
            base.Configure(builder);
        }
    }
}
