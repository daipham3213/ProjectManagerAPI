using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IGroupTypeRepository : IRepository<GroupType>
    {
        public void LoadParent(GroupType type);
        public void RemoveRelation(GroupType type);

    }
}
