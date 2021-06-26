using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class GroupTypeConfiguration : BaseConfiguration
    {
        public void Configure(EntityTypeBuilder<GroupType> builder)
        {
            //builder.HasMany(e => e.Group)
            //    .WithOne(x => x.GroupType);
        }
    }
}
