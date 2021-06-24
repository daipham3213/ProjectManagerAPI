using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class GroupTypeRepositories : Repository<GroupType>, IGroupTypeRepositories
    {
        private readonly ProjectManagerDBContext _context;
        public GroupTypeRepositories (ProjectManagerDBContext context)
            :base (context)
        {
            _context = context;
        }
        public void LoadAllParent(GroupType type)
        {
            throw new NotImplementedException();
        }

        public void RemoveRelation(GroupType type)
        {
            throw new NotImplementedException();
        }
    }
}
