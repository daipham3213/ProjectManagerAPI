using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core
{
    public class ProjectManagerDBContext : DbContext
    {
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           //Add data configuration here
        }
    }
}
