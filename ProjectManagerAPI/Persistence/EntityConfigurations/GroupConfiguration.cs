using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class GroupConfiguration : BaseConfiguration<Group>
    {
        public override void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasOne(x => x.GroupType)
                .WithMany(e => e.Group)
                .HasForeignKey(f => f.GroupTypeFk);
            builder.HasOne(u => u.ParentN).WithMany()
                .HasForeignKey(u => u.ParentNId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
            base.Configure(builder);
        }
    }
}
