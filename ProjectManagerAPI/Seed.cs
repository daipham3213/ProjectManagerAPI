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
          
            //Init Usertype
            if (context.UserTypes.Where(u => u.Name == "Dep. Leader").Count() == 0)
            {
                await roleManager.CreateAsync(roleAdmin);
                await roleManager.CreateAsync(roleUser);
                var UserTypes = new List<UserType>
                {
                    new UserType {
                        Name = "System Admin",
                        Remark = "Administrator",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        IsActived = false,
                        IsDeleted = false,
                    },
                    new UserType {
                        Name = "Dep. Leader",
                        Remark = "Department Lead",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        IsActived = true,
                        IsDeleted = false,
                    },
                    new UserType {
                        Name = "Team Leader",
                        Remark = "Team Lead",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        IsActived = true,
                        IsDeleted = false,
                    },
                    new UserType {
                        Name = "Member",
                        Remark = "Member of a group",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        IsActived = true,
                        IsDeleted = false,
                    },
                };
                for (int i = 0; i < UserTypes.Count; i++)
                {
                    var type = new UserType();
                    type = UserTypes[i];
                    if (i > 0)
                    {
                        var parent = context.UserTypes.FirstOrDefault(u => u.Name == UserTypes[i - 1].Name);
                        type.ParentN = parent;
                    }
                    await context.UserTypes.AddAsync(type);
                    await context.SaveChangesAsync();
                }
            }
            //Add some user
            if (context.Users.Where(u => u.UserName == "admin").Count() == 0)
            {
                var member = context.UserTypes.Where(u => u.Name == "Member").First();
                var admin = context.UserTypes.Where(u => u.Name == "System Admin").First();
                var users = new List<User>{
                    new User
                    {
                        UserName = "admin",
                        Name = "Administrator",
                        Bio = "System management and maintain",
                        IsActived = true,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        EmailConfirmed = true,
                        UserType = admin,
                        Email = "daipham.3123@gmail.com"
                    },
                    new User
                    {
                        UserName = "member1",
                        Name = "Member number one",
                        Bio = "First member of the system",
                        IsActived = true,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        EmailConfirmed = true,
                        UserType = member,
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
                        Name = "Department",
                        Remark = "",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        UserCreated = admin.Id,
                        IsActived = true,
                    },
                    new GroupType{
                        Name = "Team",
                        Remark = "",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        UserCreated = admin.Id,
                        IsActived = true,
                    },
                };

                context.GroupTypes.Add(types[0]);
                await context.SaveChangesAsync();
                types[1].ParentN = context.GroupTypes.FirstOrDefault(u => u.Name == types[0].Name);
                context.GroupTypes.Add(types[1]);
                await context.SaveChangesAsync();
            }
        }
    }
}
