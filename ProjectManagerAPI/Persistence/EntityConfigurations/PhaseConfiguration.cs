using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class PhaseConfiguration : BaseConfiguration
    {
        public void Configure(EntityTypeBuilder<Phase> builder)
        {
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();
        }
    }
}
