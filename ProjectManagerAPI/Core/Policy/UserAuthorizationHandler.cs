using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.Core.ServiceResource;
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
            //Edit user permission
            if (requirement.Name == UserPermissions.Edit)
            {
                var is_admin = context.User.IsInRole(RoleNames.RoleAdmin);
                if (is_admin) 
                    context.Succeed(requirement);
                else
                {
                    var username = context.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
                    if (username == user.UserName) 
                        context.Succeed(requirement);
                }
            }
            return null;
        }
    }
}
