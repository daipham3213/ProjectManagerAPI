using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class UserConfiguration : BaseConfiguration
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(u => u.UserType).WithMany(e => e.Users);
            builder.HasOne(u => u.Group)
                .WithMany(e => e.Users)
                .HasForeignKey(k => k.GroupRef);                
        }
    }
}
