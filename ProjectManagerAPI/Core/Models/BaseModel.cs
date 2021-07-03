using System;
using System.ComponentModel;

namespace ProjectManagerAPI.Core.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid UserCreated { get; set; }
        [DefaultValue(true)]
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
    }
}
