using Microsoft.AspNetCore.Identity;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ProjectManagerAPI.Core.Permission;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI
{
    public class Seed
    {
        public static async Task SeedData(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ProjectManagerDbContext context
            )
        {
            // this seed method for creating some sample data in database
            //Check if the seed is planted
            var server = context.ServerInfos.FirstOrDefault(u => u.Name == "API Server");
            if (server != null)
                if (server.IsSeeded)
                    return;

            if (server == null)
            {
                server = new ServerInfo()
                {
                    IsSeeded = true
                };
                context.ServerInfos.Add(server);
            }
            else if (server.IsSeeded == false)
                server.IsSeeded = true;

            //initalizing some roles
            var roleAdmin = new IdentityRole<Guid>(RoleNames.RoleAdmin);
            var roleUser = new IdentityRole<Guid>(RoleNames.RoleUser);
            var roleDep = new IdentityRole<Guid>(RoleNames.DepartmentLead);
            var roleTeam = new IdentityRole<Guid>(RoleNames.TeamLead);

            await roleManager.CreateAsync(roleUser);
            await roleManager.CreateAsync(roleAdmin);
            await roleManager.CreateAsync(roleDep);
            await roleManager.CreateAsync(roleTeam);

            //Init base permissions.
            //Admin Has FULL permission
            roleAdmin =await roleManager.FindByNameAsync(RoleNames.RoleUser);
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, UserPermissions.Full));
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, AvatarPermission.Full));
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, GroupTypePermission.Full));
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, GroupPermission.Full));
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, ReportPermission.Full));
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, PhasePermission.Full));
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, TaskPermission.Full));
            await roleManager.AddClaimAsync(roleAdmin, new Claim(PermissionType.Permission, ProjectPermission.Full));

            //Department leader
            roleDep = await roleManager.FindByNameAsync(RoleNames.DepartmentLead);
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, UserPermissions.FullSelf));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, AvatarPermission.FullSelf));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, GroupTypePermission.View));
        
            //Group permissions
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, GroupPermission.FullSelf));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, GroupPermission.EditLeader));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, GroupPermission.EditMember));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, ReportPermission.FullSelf));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, PhasePermission.FullSelf));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, TaskPermission.FullSelf));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, ProjectPermission.FullSelf));
            await roleManager.AddClaimAsync(roleDep, new Claim(PermissionType.Permission, ProjectPermission.View));

            //Team leader
            roleTeam = await roleManager.FindByNameAsync(RoleNames.TeamLead);
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, UserPermissions.FullSelf));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, AvatarPermission.FullSelf));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, GroupTypePermission.View));
           
            //Group permissions
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, GroupPermission.FullSelf));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, GroupPermission.EditLeader));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, GroupPermission.EditMember));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, ReportPermission.FullSelf));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, PhasePermission.FullSelf));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, TaskPermission.FullSelf));
            await roleManager.AddClaimAsync(roleTeam, new Claim(PermissionType.Permission, ProjectPermission.View));
           

            //Member
            roleUser = await roleManager.FindByNameAsync(RoleNames.TeamLead);
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, UserPermissions.FullSelf));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, AvatarPermission.FullSelf));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, GroupTypePermission.View));
       
            //Group permissions
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, GroupPermission.View));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, ReportPermission.View));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, PhasePermission.View));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, TaskPermission.View));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, TaskPermission.Edit));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, TaskPermission.Add));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, TaskPermission.RemoveSelf));
            await roleManager.AddClaimAsync(roleUser, new Claim(PermissionType.Permission, ProjectPermission.View));


            //Add users
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

            //Add some grouptypes
            var admin = await userManager.FindByNameAsync("admin");
            var types = new List<GroupType> {
                    new GroupType{
                        Name = "System Admin",
                        Remark = "System administrators and maintainers.",
                        UserCreated = admin.Id,
                        IsActived = false,
                        IdentityRole = await roleManager.FindByNameAsync(RoleNames.RoleAdmin)
                    },
                    new GroupType{
                        Name = "Department",
                        Remark = "Big group that consist of smaller related group.",
                        UserCreated = admin.Id,
                        IsActived = true,
                        IdentityRole = await roleManager.FindByNameAsync(RoleNames.DepartmentLead)
                    },
                    new GroupType{
                        Name = "Group",
                        Remark = "Small group of people for specified works.",
                        UserCreated = admin.Id,
                        IsActived = true,
                        IdentityRole = await roleManager.FindByNameAsync(RoleNames.TeamLead)
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
