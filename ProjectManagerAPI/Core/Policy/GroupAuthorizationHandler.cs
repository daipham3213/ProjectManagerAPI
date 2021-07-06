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
            
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);
            var isLeader = await IsLeader(context.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value);
            if (isAdmin)
                context.Succeed(requirement);
            var groups = await GetValidatedGroups(context);
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
            //Read
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

        private async Task<IEnumerable<Group>> GetValidatedGroups(AuthorizationHandlerContext context)
        {
            var username = context.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
            var user = await this._unitOfWork.Users.GetUser(username);
            var leader = user.ParentN?.Id ?? user.Id;
            var groups = await this._unitOfWork.Groups.GetGroupListValidated(leader);
            return groups;
        }

        private async Task<bool> IsLeader(string username)
        {
            if (username == null)
                return false;
            var user = await this._unitOfWork.Users.GetUser(username);
            var group = await this._unitOfWork.Groups.GetGroupByLeaderId(user.Id);
            if (group == null)
                return false;
            return true;
        }
    }
}
