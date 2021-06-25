using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class UsertypeRepository : Repository<UserType>, IUserTypeRepository
    {
        private readonly ProjectManagerDBContext _context;

        public UsertypeRepository(ProjectManagerDBContext context)
            : base(context)
        {
            _context = context;
        }

        public void LoadParent(GroupType type)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GroupType>> LoadValidated()
        {
            throw new NotImplementedException();
        }

        public void RemoveRelation(GroupType type)
        {
            throw new NotImplementedException();
        }
    }
}
