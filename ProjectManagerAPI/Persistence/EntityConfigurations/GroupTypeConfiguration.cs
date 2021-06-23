using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
