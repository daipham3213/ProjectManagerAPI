using ProjectManagerAPI.Core.Models;
using System;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Services
{
    public interface IGroupTypeService
    {
        public Task<GroupType> GetParents(Guid postId);
    }
}
