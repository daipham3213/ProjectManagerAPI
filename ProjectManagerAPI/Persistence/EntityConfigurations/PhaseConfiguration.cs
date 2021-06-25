using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class PhaseConfiguration : BaseConfiguration
    {
        public void Configure(EntityTypeBuilder<Phase> builder)
        {
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();
        }
    }
}
