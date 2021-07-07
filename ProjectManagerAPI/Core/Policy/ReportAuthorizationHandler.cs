using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.Core.ServiceResource;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Policy
{
    public class ReportAuthorizationHandler 
        :  AuthorizationHandler<OperationAuthorizationRequirement, Report>
    {
        public ReportAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            Report resource)
        {
            var Utils = new AuthorizeUtils(this._unitOfWork);
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);
            var isLeader = await Utils.IsLeader(context.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value);
            if (isAdmin)
                context.Succeed(requirement);
            var groups = await Utils.GetValidatedGroups(context);
            //Check if user has full permission
            if (context.User.HasClaim(u => u.Value.Equals(ReportPermission.Full)))
                context.Succeed(requirement);
            //Check if user has FULL Self permission
            if (context.User.HasClaim(u => u.Value.Equals(ReportPermission.FullSelf))
                & !GroupPermission.SpecialPerm.Contains(requirement.Name))
            {
                
            }
        }
    }
}
