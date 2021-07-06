using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Permission
{
    public static class ReportPermission
    {
        //C R U D reports
        public const string Add = "reports.add";
        public const string Edit = "reports.edit";
        public const string Remove = "reports.remove";
        public const string View = "reports.view";
        public const string Full = "reports.full";
        public const string FullSelf = "report.full.self";
        public static List<string> SpecialPerm = new List<string>()
        {
            Full
        };
    }
}