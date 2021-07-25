using System;

namespace ProjectManagerAPI.Core.Models
{
    public class Avatar
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string PublicId { get; set; }
        public DateTime UploadTime { get; set; }
        public bool IsMain { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
