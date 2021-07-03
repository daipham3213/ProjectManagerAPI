using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IGroupTypeRepository : IRepository<GroupType>
    {
        public void LoadParent(GroupType type);
        public void RemoveRelation(GroupType type);
        public Task<IEnumerable<GroupType>> LoadValidated();
        public Task<GroupType> GetTypeByName(string name);
        void RemoveAllChildren(Guid groupId);
        public Task<GroupType> GetParents(Guid postId);
    }
}
