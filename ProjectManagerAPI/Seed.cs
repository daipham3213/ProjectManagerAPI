using Microsoft.AspNetCore.Identity;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ProjectManagerAPI
{
    public class Seed
    {
        public static async System.Threading.Tasks.Task SeedData(
            UserManager<User> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager,
            ProjectManagerDBContext context
            )
        {
            // this seed method for creating some sample data in database

            //initalizing some roles
            var roleAdmin = new IdentityRole<Guid>(RoleNames.RoleAdmin);
            var roleUser = new IdentityRole<Guid>(RoleNames.RoleUser);
            
            //Add some user
            if (context.Users.Where(u => u.UserName == "admin").Count() == 0)
            {
                await roleManager.CreateAsync(roleUser);
                await roleManager.CreateAsync(roleAdmin);
                var users = new List<User>{
                    new User
                    {
                        UserName = "admin",
                        Name = "Administrator",
                        Bio = "System management and maintain",
                        IsActived = true,  
                        EmailConfirmed = true,
                        IsSuperuser = true,
                        Email = "daipham.3123@gmail.com"
                    },
                    new User
                    {
                        UserName = "member1",
                        Name = "Member number one",
                        Bio = "First member of the system",
                        IsActived = true,
                        EmailConfirmed = true,
                        Email = "cute200052@gmail.com"
                    }
                };

                await userManager.CreateAsync(users[0], "admin123456");
                await userManager.AddToRoleAsync(users[0], roleAdmin.Name);

                await userManager.CreateAsync(users[1], "member123456");
                await userManager.AddToRoleAsync(users[1], roleUser.Name);
            }
            //Add some grouptypes
            if (context.GroupTypes.Where(u => u.Name == "Department").Count() == 0)
            {
                var admin =await userManager.FindByNameAsync("admin");
                var types = new List<GroupType> {
                    new GroupType{
                        Name = "System Admin",
                        Remark = "",
                        UserCreated = admin.Id,
                        IsActived = true,
                    },
                    new GroupType{
                        Name = "Department",
                        Remark = "",
                        UserCreated = admin.Id,
                        IsActived = true,
                    },
                    new GroupType{
                        Name = "Team",
                        Remark = "",
                        UserCreated = admin.Id,
                        IsActived = true,
                    },
                };

                context.GroupTypes.Add(types[0]);
                await context.SaveChangesAsync();
                types[1].ParentN = context.GroupTypes.FirstOrDefault(u => u.Name == types[0].Name);
                context.GroupTypes.Add(types[1]);
                await context.SaveChangesAsync();
                types[2].ParentN = context.GroupTypes.FirstOrDefault(u => u.Name == types[1].Name);
                context.GroupTypes.Add(types[2]);
                await context.SaveChangesAsync();
            }
        }
    }
}
