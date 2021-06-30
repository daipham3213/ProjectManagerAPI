using System;

namespace ProjectManagerAPI.Core.Resources
{
    public class AvatarResource
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public DateTime UploadTime { get; set; }
        public int IsMain { get; set; }
    }
}
