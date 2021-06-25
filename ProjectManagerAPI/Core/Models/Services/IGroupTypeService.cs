using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Services
{
    public interface IGroupTypeService
    {
        public Task<GroupType> GetParents(Guid postId);
    }
}
