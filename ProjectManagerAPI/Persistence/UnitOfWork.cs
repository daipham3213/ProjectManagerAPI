using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectManagerDBContext _context;

        public UnitOfWork(ProjectManagerDBContext context, IGroupTypeRepository groupType)
        {
            _context = context;
            GroupType = groupType;
        }

        public IGroupTypeRepository GroupType { get; private set; }

        public async Task<int> Complete()
        {
            return await this._context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._context.Dispose();
        }
    }
}
