using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Permission
{
    public static class TaskPermission
    {
        //C R U D tasks
        public const string Add = "tasks.add";
        public const string Edit = "tasks.edit";
        public const string Remove = "tasks.remove";
        public const string RemoveSelf = "tasks.remove.self";
        public const string View = "tasks.view";
        public const string Full = "tasks.full";
        public const string FullSelf = "task.full.self";
        public static List<string> SpecialPerm = new List<string>()
        {
            Full
        };
    }
}