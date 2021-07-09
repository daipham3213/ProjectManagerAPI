using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Policy
{
    public class PhaseAuthorizationHandler 
        : AuthorizationHandler<OperationAuthorizationRequirement, Phase>
    {

        private readonly IUnitOfWork _unitOfWork;

        public PhaseAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<IAsyncResult> HandleRequirementAsync(AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            Phase resource)
        {

            var Utils = new AuthorizeUtils(this._unitOfWork);
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);
          
            if (isAdmin)
                context.Succeed(requirement);
            var groups = await Utils.GetValidatedGroups(context);
            //Check if user has full permission
            if (context.User.HasClaim(u => u.Value.Equals(PhasePermission.Full)))
                context.Succeed(requirement);

            if (context.User.HasClaim(u => u.Value.Equals(ProjectPermission.FullSelf)))
                context.Succeed(requirement);


            //Edit leader and TeamLead
            //create leader and TeamLead
            //detele leader and TeamLead
            if (requirement.Name == PhasePermission.Edit
           | requirement.Name == PhasePermission.Add
           | requirement.Name == PhasePermission.Remove)
            {
             //   if (context.User.IsInRole(RoleNames.RoleUser))
                    if (context.User.IsInRole(RoleNames.TeamLead) | context.User.IsInRole(RoleNames.DepartmentLead))
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}
