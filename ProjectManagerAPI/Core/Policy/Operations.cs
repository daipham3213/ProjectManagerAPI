using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProjectManagerAPI.Core.Permission;

namespace ProjectManagerAPI.Core.Policy
{
    public static class Operations
    {
        //Group
        public static OperationAuthorizationRequirement GroupCreate =
            new OperationAuthorizationRequirement { Name = GroupPermission.Add };
        public static OperationAuthorizationRequirement GroupRead =
            new OperationAuthorizationRequirement { Name = GroupPermission.View };
        public static OperationAuthorizationRequirement GroupUpdate =
            new OperationAuthorizationRequirement { Name = GroupPermission.Edit };
        public static OperationAuthorizationRequirement GroupLeaderUpdate =
            new OperationAuthorizationRequirement { Name = GroupPermission.EditLeader };
        public static OperationAuthorizationRequirement GroupMemberUpdate =
            new OperationAuthorizationRequirement { Name = GroupPermission.EditMember };
        public static OperationAuthorizationRequirement GroupDelete =
            new OperationAuthorizationRequirement { Name = GroupPermission.Remove};
    }
}
