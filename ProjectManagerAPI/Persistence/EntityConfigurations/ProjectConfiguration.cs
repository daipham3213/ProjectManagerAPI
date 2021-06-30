using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class ProjectConfiguration : BaseConfiguration<Project>
    {
        public override void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();
            base.Configure(builder);
        }
    }
}
