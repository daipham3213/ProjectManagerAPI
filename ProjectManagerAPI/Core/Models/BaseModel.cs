using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class BaseModel
    {
        [Key]
        public Guid id { get; set; }
        public string name { get; set; }
        public string remark { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_modified { get; set; }
        public Guid user_created { get; set; }
        public bool is_active { get; set; }
        public bool is_delete { get; set; }
    }
}
