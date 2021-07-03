using System;

namespace ProjectManagerAPI.Core.Models
{
    public class ServerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSeeded { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
