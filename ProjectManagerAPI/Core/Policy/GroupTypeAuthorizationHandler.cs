using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.Core.ServiceResource;
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
            //Admin could do all
            if (context.User.IsInRole(RoleNames.RoleAdmin))
                context.Succeed(requirement);
            //Check Full Perm
            if (context.User.HasClaim(u => u.Value == GroupTypePermission.Full))
                context.Succeed(requirement);
            //View Permission
            if (requirement.Name == GroupTypePermission.View
                & context.User.HasClaim(u => u.Value == GroupTypePermission.View))
            {
                context.Succeed(requirement);
            }
            //Add
            if (requirement.Name == GroupTypePermission.Add
                & context.User.HasClaim(u => u.Value == GroupTypePermission.Add))
            {
                context.Succeed(requirement);
            }
            //Edit
            if (requirement.Name == GroupTypePermission.Edit
                & context.User.HasClaim(u => u.Value == GroupTypePermission.Edit))
            {
                context.Succeed(requirement);
            }
            //Delete
            if (requirement.Name == GroupTypePermission.Remove
                & context.User.HasClaim(u => u.Value == GroupTypePermission.Remove))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
