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
using ProjectManagerAPI.StaticValue;
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
        protected override async Task<IAsyncResult> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            Report resource)
        {
            var utils = new AuthorizeUtils(this._unitOfWork);
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);
            if (isAdmin)
                context.Succeed(requirement);
            var groups = await utils.GetValidatedGroups(context);
            var isLeader = await utils.IsLeader(context.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value);
            //Check if user has full permission
            if (context.User.HasClaim(u => u.Value.Equals(ReportPermission.Full)))
                context.Succeed(requirement);
            //Check if user has FULL Self permission
            if (context.User.HasClaim(u => u.Value.Equals(ReportPermission.FullSelf))
                & !ReportPermission.SpecialPerm.Contains(requirement.Name))
            {
                if (isLeader)
                {
                    if (groups.Any(@group => @group.Id == resource.GroupId))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            //Create
            if (requirement.Name == ReportPermission.Add)
            {
                if (isLeader & context.User.HasClaim(u => u.Value == resource.GroupId.ToString()))
                    context.Succeed(requirement);
            }
            //Read
            if (requirement.Name == ReportPermission.View)
            {
                if (isLeader)
                {
                    //leader can view all report from child groups too
                    if (groups.Any(@group => @group.Id == resource.GroupId))
                    {
                        context.Succeed(requirement);
                    }
                }
                else
                {
                    //if not leader can only view report in their group
                    if (context.User.HasClaim(u => u.Value == resource.GroupId.ToString()))
                        context.Succeed(requirement);
                }
            }

            //Update 
            //Delete 
            if (requirement.Name == ReportPermission.Edit 
                | requirement.Name == ReportPermission.Remove)
            {
                if (isLeader)
                {
                    //leader can view all report from child groups too
                    if (groups.Any(@group => @group.Id == resource.GroupId))
                    {
                        context.Succeed(requirement);
                    }
                }
                else 
                if(context.User.HasClaim(u => u.Value == requirement.Name)
                   & context.User.HasClaim(u => u.Value == resource.GroupId.ToString()))
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}
