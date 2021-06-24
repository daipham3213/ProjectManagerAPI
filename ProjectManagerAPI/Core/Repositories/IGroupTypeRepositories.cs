using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    interface IGroupTypeRepositories : IRepositoires<GroupType>
    {
        public void LoadAllParent(GroupType type);
        public void RemoveRelation(GroupType type);

    }
}
