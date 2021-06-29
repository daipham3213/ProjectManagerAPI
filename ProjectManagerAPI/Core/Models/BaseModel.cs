using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagerAPI.Core.Models
{
    public class BaseModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }        
        public DateTime? DateCreated { get; set; } 
        public DateTime? DateModified { get; set; }
        public Guid UserCreated { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
    }
}
