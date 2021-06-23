using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class ReportConfiguration : BaseConfiguration
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.Property(u => u.StartDate).ValueGeneratedOnAdd();
            builder.Property(u => u.DueDate).ValueGeneratedOnAdd();
            
            builder.HasOne(p => p.Project).WithMany(r => r.Reports)
                .HasForeignKey(k => k.ProjectID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Group).WithMany(r => r.Reports)
                .HasForeignKey(k => k.GroupID);
        }
    }
}
