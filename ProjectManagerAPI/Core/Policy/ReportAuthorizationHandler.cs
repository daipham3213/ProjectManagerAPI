﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Policy
{
    public class ReportAuthorizationHandler 
        :  AuthorizationHandler<OperationAuthorizationRequirement, Report>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            Report resource)
        {
            throw new NotImplementedException();
        }
    }
}
