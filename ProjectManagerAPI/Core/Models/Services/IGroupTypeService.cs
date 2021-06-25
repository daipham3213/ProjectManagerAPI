using System;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Services
{
    public interface IGroupTypeService
    {
        public Task<GroupType> GetParents(Guid postId);
    }
}
