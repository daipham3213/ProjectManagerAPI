using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public abstract class BaseConfiguration<TBase> : IEntityTypeConfiguration<TBase>
        where TBase : BaseModel
    {
        public virtual void Configure(EntityTypeBuilder<TBase> builder)
        {
            builder.HasKey(k => k.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(x => x.DateCreated)
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Name);
            builder.Property(x => x.DateModified)
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAddOrUpdate();
            //builder.Property(x => x.IsActived).HasDefaultValue(true);
        }
    }
}
