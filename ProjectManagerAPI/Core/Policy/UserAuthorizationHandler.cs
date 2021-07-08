using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Policy
{
    public class UserAuthorizationHandler : 
        AuthorizationHandler<OperationAuthorizationRequirement, User>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            User user
            )
        {
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);
            if (isAdmin)
                context.Succeed(requirement);
            //Check if user has full permission
            if (context.User.HasClaim(u => u.Value.Equals(UserPermissions.Full)))
                context.Succeed(requirement);
            //Check if user has FULL Self permission
            if (context.User.HasClaim(u => u.Value.Equals(UserPermissions.FullSelf))
                & !UserPermissions.SpecialPerm.Contains(requirement.Name))
            {
                if (context.User.Claims.Any(u => u.Value ==user.UserName & u.Type == ClaimTypes.Name))
                    context.Succeed(requirement);
            }
            //Edit
            if (requirement.Name == UserPermissions.Edit)
            {
                if (context.User.HasClaim(u => u.Value == user.UserName)) 
                        context.Succeed(requirement);
            }

            //Delete
            if (requirement.Name == UserPermissions.Delete)
            {
                if (context.User.HasClaim(u => u.Value == UserPermissions.Delete))
                {
                        context.Succeed(requirement);
                }
            }
            //Edit role
            if (requirement.Name == UserPermissions.EditRole)
            {
                if (context.User.HasClaim(u => u.Value == UserPermissions.EditRole))
                {
                    context.Succeed(requirement);
                }
            }
            //View 
            if (requirement.Name == UserPermissions.View 
                & context.User.HasClaim(u => u.Value == UserPermissions.View))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }


    }
}
