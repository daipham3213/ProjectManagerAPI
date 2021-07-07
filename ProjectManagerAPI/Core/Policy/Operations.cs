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

        //User
        public static OperationAuthorizationRequirement UserCreate =
            new OperationAuthorizationRequirement { Name = UserPermissions.Add };
        public static OperationAuthorizationRequirement UserRead =
            new OperationAuthorizationRequirement { Name = UserPermissions.View };
        public static OperationAuthorizationRequirement UserUpdate =
            new OperationAuthorizationRequirement { Name = UserPermissions.Edit };
        public static OperationAuthorizationRequirement UserRoleUpdate =
            new OperationAuthorizationRequirement { Name = UserPermissions.EditRole };
        public static OperationAuthorizationRequirement UserDelete =
            new OperationAuthorizationRequirement { Name = UserPermissions.Delete };

        //Avatar
        public static OperationAuthorizationRequirement AvatarCreate =
            new OperationAuthorizationRequirement { Name = AvatarPermission.Add };
        public static OperationAuthorizationRequirement AvatarRead =
            new OperationAuthorizationRequirement { Name = AvatarPermission.View };
        public static OperationAuthorizationRequirement AvatarUpdate =
            new OperationAuthorizationRequirement { Name = AvatarPermission.Edit };
        public static OperationAuthorizationRequirement AvatarDelete =
            new OperationAuthorizationRequirement { Name = AvatarPermission.Remove };

        //GroupType
        public static OperationAuthorizationRequirement GroupTypeCreate =
            new OperationAuthorizationRequirement { Name = GroupTypePermission.Add };
        public static OperationAuthorizationRequirement GroupTypeRead =
            new OperationAuthorizationRequirement { Name = GroupTypePermission.View };
        public static OperationAuthorizationRequirement GroupTypeUpdate =
            new OperationAuthorizationRequirement { Name = GroupTypePermission.Edit };
        public static OperationAuthorizationRequirement GroupTypeDelete =
            new OperationAuthorizationRequirement { Name = GroupTypePermission.Remove };

        //Project
        public static OperationAuthorizationRequirement ProjectCreate =
            new OperationAuthorizationRequirement { Name = ProjectPermission.Add };
        public static OperationAuthorizationRequirement ProjectRead =
            new OperationAuthorizationRequirement { Name = ProjectPermission.View };
        public static OperationAuthorizationRequirement ProjectUpdate =
            new OperationAuthorizationRequirement { Name = ProjectPermission.Edit };
        public static OperationAuthorizationRequirement ProjectDelete =
            new OperationAuthorizationRequirement { Name = ProjectPermission.Remove };

        //Report
        public static OperationAuthorizationRequirement ReportCreate =
            new OperationAuthorizationRequirement { Name = ReportPermission.Add };
        public static OperationAuthorizationRequirement ReportRead =
            new OperationAuthorizationRequirement { Name = ReportPermission.View };
        public static OperationAuthorizationRequirement ReportUpdate =
            new OperationAuthorizationRequirement { Name = ReportPermission.Edit };
        public static OperationAuthorizationRequirement ReportDelete =
            new OperationAuthorizationRequirement { Name = ReportPermission.Remove };

        //Phase
        public static OperationAuthorizationRequirement PhaseCreate =
            new OperationAuthorizationRequirement { Name = PhasePermission.Add };
        public static OperationAuthorizationRequirement PhaseRead =
            new OperationAuthorizationRequirement { Name = PhasePermission.View };
        public static OperationAuthorizationRequirement PhaseUpdate =
            new OperationAuthorizationRequirement { Name = PhasePermission.Edit };
        public static OperationAuthorizationRequirement PhaseDelete =
            new OperationAuthorizationRequirement { Name = PhasePermission.Remove };

        //Task
        public static OperationAuthorizationRequirement TaskCreate =
            new OperationAuthorizationRequirement { Name = TaskPermission.Add };
        public static OperationAuthorizationRequirement TaskRead =
            new OperationAuthorizationRequirement { Name = TaskPermission.View };
        public static OperationAuthorizationRequirement TaskUpdate =
            new OperationAuthorizationRequirement { Name = TaskPermission.Edit };
        public static OperationAuthorizationRequirement TaskDelete =
            new OperationAuthorizationRequirement { Name = TaskPermission.Remove };
        public static OperationAuthorizationRequirement TaskSelfDelete =
            new OperationAuthorizationRequirement { Name = TaskPermission.RemoveSelf };
    }
}
