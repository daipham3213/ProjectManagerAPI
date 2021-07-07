using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ProjectManagerAPI.Core.Policy
{
    public class TaskAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Task>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            Task resource)
        {
            throw new NotImplementedException();
        }
    }
}
