﻿using System;
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
    public class ProjectAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Project>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectAuthorizationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<IAsyncResult> HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Project resource)
        {
            var Utils = new AuthorizeUtils(this._unitOfWork);
            var isAdmin = context.User.IsInRole(RoleNames.RoleAdmin);
            var isLeader = await Utils.IsLeader(context.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value);

          
            if (isAdmin) 
                context.Succeed(requirement);
            //Check if user has full permission
            if (context.User.HasClaim(u => u.Value.Equals(ProjectPermission.Full)))
                context.Succeed(requirement);
            //Check if user has FULL Self permission
            if (context.User.HasClaim(u => u.Value.Equals(ProjectPermission.FullSelf)))
                context.Succeed(requirement);

            //Read Permission
            if (requirement.Name == ProjectPermission.View & context.User.HasClaim(u => u.Value == requirement.Name))
            {         
                    context.Succeed(requirement);
            }

            //Edit leader
            //create leader
            //detele leader
            if (requirement.Name == ProjectPermission.Edit
                | requirement.Name == ProjectPermission.Add
                | requirement.Name == ProjectPermission.Remove)
            {

                if (isLeader | context.User.IsInRole(RoleNames.DepartmentLead)) 

                    context.Succeed(requirement);
            }

            return Task.CompletedTask;

   

        }
    }
}