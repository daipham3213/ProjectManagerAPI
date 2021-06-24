﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.EntityConfigurations
{
    public class BaseConfiguration : IEntityTypeConfiguration<BaseModel>
    {
        public void Configure(EntityTypeBuilder<BaseModel> builder)
        {
            builder.HasKey(k => k.ID);
            builder.Property(x => x.ID).ValueGeneratedOnAdd()
                .IsRequired();
            builder.Property(x => x.DateCreated).ValueGeneratedOnAdd();
            builder.Property(x => x.DateModified).ValueGeneratedOnAddOrUpdate();
        }
    }
}