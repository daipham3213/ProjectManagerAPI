using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Policy
{
    public class GroupTypeAuthorizationHandler:
        AuthorizationHandler<OperationAuthorizationRequirement, GroupType>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            GroupType resource)
        {
            //Admin or has Full perm could do all
            if (context.User.IsInRole(RoleNames.RoleAdmin) 
                | context.User.HasClaim(u => u.Value == GroupTypePermission.Full))
                context.Succeed(requirement);

            //View Permission
            if (requirement.Name == GroupTypePermission.View
                & context.User.HasClaim(u => u.Value == requirement.Name))
            {
                context.Succeed(requirement);
            }
            //Add Edit Delete
            if ((requirement.Name == GroupTypePermission.Add 
                | requirement.Name == GroupTypePermission.Edit 
                | requirement.Name == GroupTypePermission.Remove)  & context.User.HasClaim(u => u.Value == requirement.Name))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
