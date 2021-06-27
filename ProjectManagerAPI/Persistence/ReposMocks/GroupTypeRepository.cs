using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
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

        public async Task<GroupType> GetTypeByName(string name)
        {
            return await this._context.GroupTypes.FirstOrDefaultAsync(u => u.Name == name);
        }

        public void LoadParent(GroupType type)
        {
            this._context.GroupTypes.Where(u => u.ParentN.ID == type.ID).Load();
            if (type.ParentN != null)
            {
                LoadParent(type.ParentN);
            }
        }

        public async Task<IEnumerable<GroupType>> LoadValidated()
        {
            return await this._context.GroupTypes.Where(u => u.IsDeleted == false).ToListAsync();
        }

        public void RemoveAllChildren(Guid typeid)
        {
            var type = this._context.GroupTypes.Find(typeid);
            var child = this._context.GroupTypes.FirstOrDefault(u => u.ParentN != null & type.ID == u.ParentN.ID);

            if (child == null)
            {
                this._context.RemoveRange(type);
                return;
            }
            RemoveAllChildren(child.ID);
        }

        public void RemoveRelation(GroupType type)
        {
            var t = this._context.GroupTypes.Find(type);
            t.ParentN = null;
            _context.SaveChanges();
        }
    }
}
