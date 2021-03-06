using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class GroupTypeConfiguration : BaseConfiguration<GroupType>
    {
        public override void Configure(EntityTypeBuilder<GroupType> builder)
        {
            base.Configure(builder);
            builder.HasOne(u => u.IdentityRole)
                .WithMany()
                .HasForeignKey(f => f.IdentityRoleId);
        }
    }
}
