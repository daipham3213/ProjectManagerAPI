using Microsoft.AspNetCore.Identity;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Models.ServiceResource;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ProjectManagerAPI
{
    public class Seed
    {
        public static async System.Threading.Tasks.Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            // this seed method for creating some sample data in database

            //initalizing some roles
            var roleAdmin = new IdentityRole<Guid>(RoleNames.RoleAdmin);
            var roleUser = new IdentityRole<Guid>(RoleNames.RoleUser);

            await roleManager.CreateAsync(roleAdmin);
            await roleManager.CreateAsync(roleUser);
        }
    }
}
