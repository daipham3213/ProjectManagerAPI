using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class GroupTypeRepository : Repository<GroupType>, IGroupTypeRepository
    {
        private readonly ProjectManagerDBContext _context;
        public GroupTypeRepository(ProjectManagerDBContext context)
            : base(context)
        {
            _context = context;
        }

        public void LoadParent(GroupType type)
        {
            this._context.GroupTypes.Where(u => u.ParentN.ID == type.ID).Load();
            if (type.ParentN == null)
            {
                return;
            }
            LoadParent(type.ParentN);
        }

        public async Task<IEnumerable<GroupType>> LoadValidated()
        {
            return await this._context.GroupTypes.Where(u => u.IsDeleted == false).ToListAsync();
        }

        public void RemoveRelation(GroupType type)
        {
            var t = this._context.GroupTypes.Find(type);
            t.ParentN = null;
            _context.SaveChanges();
        }
    }
}
