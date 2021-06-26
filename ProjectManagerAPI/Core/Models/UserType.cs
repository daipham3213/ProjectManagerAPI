using System.Collections.Generic;

namespace ProjectManagerAPI.Core.Models
{
    public class UserType : BaseModel
    {
        public UserType? ParentN { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
