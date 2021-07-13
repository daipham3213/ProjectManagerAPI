using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Permission;
using ProjectManagerAPI.StaticValue;

namespace ProjectManagerAPI.Core.Policy
{
    public class TaskAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Task>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<IAsyncResult> HandleRequirementAsync(AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            Task resource)
        {
            var utils = new AuthorizeUtils(this._unitOfWork);
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);

            if (isAdmin)
                context.Succeed(requirement);

            if (context.User.HasClaim(u => u.Value.Equals(ReportPermission.Full)))
                context.Succeed(requirement);

            if (context.User.HasClaim(u => u.Value.Equals(TaskPermission.FullSelf)) & !TaskPermission.SpecialPerm.Contains(requirement.Name))
                context.Succeed(requirement);


            if (requirement.Name == TaskPermission.View & context.User.HasClaim(u => u.Value == requirement.Name))
            {
                         context.Succeed(requirement);
            }

            // team lead and deplead add
            //  team lead and deplead edit
            // team lead and deplead remost
            if (requirement.Name == TaskPermission.Edit
                | requirement.Name == TaskPermission.Add
                | requirement.Name == TaskPermission.Remove)
            {
             
                    if (context.User.IsInRole(RoleNames.TeamLead) | context.User.IsInRole(RoleNames.DepartmentLead))
                        context.Succeed(requirement);
            }

            // user removeSeft
         
            if (requirement.Name == TaskPermission.RemoveSelf) {
                if (context.User.IsInRole(RoleNames.RoleUser))
                    context.Succeed(requirement);
            }

           
            return Task.CompletedTask;




        }
    }
}
