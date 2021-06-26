using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class GroupConfiguration : BaseConfiguration
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasOne(x => x.GroupType)
                .WithMany(e => e.Group)
                .HasForeignKey(f => f.GroupTypeFK);
        }
    }
}
