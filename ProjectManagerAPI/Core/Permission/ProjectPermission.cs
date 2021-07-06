using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Permission
{
    public static class ProjectPermission
    {
        //C R U D projects
        public const string Add = "projects.add";
        public const string Edit = "projects.edit";
        public const string Remove = "projects.remove";
        public const string View = "projects.view";
        public const string Full = "projects.full";
        public const string FullSelf = "projects.full.self";
        public static List<string> SpecialPerm = new List<string>()
        {
            Full, Add
        };
    }
}