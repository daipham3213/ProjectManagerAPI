using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // builder.HasOne(u => u.User)
            //     .WithMany(u => u.RefreshTokens)
            //     .HasForeignKey(u => u.UserId);
        }
    }
}
