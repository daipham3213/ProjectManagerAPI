using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Permission
{
    public static class GroupPermission
    {
        //C R U D groups
        public const string Add = "groups.add";
        public const string Edit = "groups.edit";
        public const string EditLeader = "groups.edit.leader";
        public const string EditMember = "groups.edit.member";
        public const string Remove = "groups.remove";
        public const string View = "groups.view";
        public const string Full = "groups.full";
        public const string FullSelf = "groups.full.self";

        public static List<string> SpecialPerm = new List<string>()
        {
            Full, Remove, Add
        };
    }
}