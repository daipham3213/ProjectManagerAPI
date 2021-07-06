using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Permission
{
    public static partial class UserPermissions
    {
        public const string Add = "users.add";
        public const string Edit = "users.edit";
        public const string View = "user.view";
        public const string Full = "user.full";
        public const string FullSelf = "user.full.self";
        public const string EditRole = "users.edit.role";
        public const string Delete = "users.delete";

        public static readonly List<string> SpecialPerm = new List<string>()
        {
            Full, Delete, Add, EditRole
        };
    }
}