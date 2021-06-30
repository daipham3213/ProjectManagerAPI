using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class PhaseConfiguration : BaseConfiguration<Phase>
    {
        public override void Configure(EntityTypeBuilder<Phase> builder)
        {
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();
            base.Configure(builder);
        }
    }
}
