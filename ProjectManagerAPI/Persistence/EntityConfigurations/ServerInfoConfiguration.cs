using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class ServerInfoConfiguration : IEntityTypeConfiguration<ServerInfo>
    {
        public void Configure(EntityTypeBuilder<ServerInfo> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name).IsRequired(true);
            builder.HasIndex(u => u.Name).IsUnique(true);
            builder.Property(u => u.IsSeeded).HasDefaultValue(false);
            builder.Property(u => u.Name).HasDefaultValue("API Server");
            builder.Property(u => u.CreateDate).HasDefaultValue(DateTime.UtcNow);
            builder.Property(u => u.UpdateDate).HasDefaultValue(DateTime.UtcNow);
        }
    }
}
