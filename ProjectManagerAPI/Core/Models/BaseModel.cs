using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{

    public class BaseModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? Remark { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid UserCreated { get; set; }
        public bool IsActived { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
