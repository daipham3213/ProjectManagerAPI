using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models
{
    public class Project : BaseModel
    {
        public ICollection<Report> Reports { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
