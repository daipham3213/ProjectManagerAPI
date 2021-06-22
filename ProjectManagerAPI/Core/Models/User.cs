using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class User : BaseModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public virtual UserType UserType { get; set; }
    }
}
