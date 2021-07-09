using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Policy
{
    public class GroupAuthorizationHandler 
        : AuthorizationHandler<OperationAuthorizationRequirement, Group>
    {
        public GroupAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;
        
        protected override async Task<IAsyncResult> HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Group resource)
        {
            var Utils = new AuthorizeUtils(this._unitOfWork);
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);
            var isLeader = await Utils.IsLeader(context.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value);
            if (isAdmin)
                context.Succeed(requirement);
            var groups = await Utils.GetValidatedGroups(context);
            //Check if user has full permission
            if (context.User.HasClaim(u => u.Value.Equals(GroupPermission.Full)))
                context.Succeed(requirement);
            //Check if user has FULL Self permission
            if (context.User.HasClaim(u => u.Value.Equals(GroupPermission.FullSelf))
                & !GroupPermission.SpecialPerm.Contains(requirement.Name))
            {
                // if (context.User.HasClaim(u => u.Value == resource.LeaderId.ToString()))
                //     context.Succeed(requirement);
                if (groups.Contains(resource))
                    context.Succeed(requirement);
            }

            //Create
            if (requirement.Name == GroupPermission.Add
                //& context.User.HasClaim(u => u.Value == GroupPermission.Add)
                )
            {
                if (context.User.Claims.Any(u => u.Type == "groupId" & u.Value == Guid.Empty.ToString())
                    & (resource.LeaderId == null | resource.LeaderId.Equals(Guid.Empty)))
                    context.Succeed(requirement);
                else
                {
                    var leader = await this._unitOfWork.Users.SearchUserById(resource.LeaderId);
                        if (leader is { GroupRef: null }) context.Succeed(requirement);
                }
            }
         
            if (requirement.Name == GroupPermission.View)
            {
                if (groups.Contains(resource))
                    context.Succeed(requirement);
            }
            //Update
            if (requirement.Name == GroupPermission.Edit)
            {
                if (groups.Contains(resource) & 
                    context.User.HasClaim(
                        u => u.Value == resource.LeaderId.ToString()
                        & u.Type == "ID")
                    )
                    context.Succeed(requirement);
            }
            //Delete
            //Edit Leader
            //Edit Member
            if (requirement.Name == GroupPermission.EditMember 
                | requirement.Name == GroupPermission.EditLeader 
                | requirement.Name == GroupPermission.Remove)
            {
                if (resource != null & groups.Contains(resource) & 
                    (isLeader | context.User.HasClaim(u => u.Value.Equals(requirement.Name)))
                    )
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        
    }
}
