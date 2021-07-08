using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Policy
{
    public class RequestAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Request>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Request resource)
        {
            if (context.User.IsInRole(RoleNames.RoleAdmin))
                context.Succeed(requirement);
            if (requirement.Name == RequestPermission.ActiveGroup)
            {
                var is_admin = context.User.HasClaim(u => u.Value == resource.To.ToString());
                if (is_admin)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
