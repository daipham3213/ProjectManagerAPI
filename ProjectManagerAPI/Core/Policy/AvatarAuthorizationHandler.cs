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
    public class AvatarAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Avatar>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            Avatar resource)
        {
            if (context.User.IsInRole(RoleNames.RoleAdmin))
                context.Succeed(requirement);
            if (requirement.Name == AvatarPermission.Remove)
            {
                if (context.User.HasClaim(u => u.Value == requirement.Name | u.Value == resource.UserId.ToString()))
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
