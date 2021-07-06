using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Permission
{
    public static class AvatarPermission
    {
        public const string Add = "avatars.add";
        public const string Edit = "avatars.edit";
        public const string Remove = "avatars.remove";
        public const string View = "avatars.view";
        public const string Full = "avatars.full";
        public const string FullSelf = "avatars.full.self";

        public static List<string> SpecialPerm = new List<string>()
        {
            Full
        };
    }
}