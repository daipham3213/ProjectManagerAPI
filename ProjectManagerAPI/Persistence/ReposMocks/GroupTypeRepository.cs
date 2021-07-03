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
        private readonly ProjectManagerDbContext _context;
        public GroupTypeRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<GroupType> GetTypeByName(string name)
        {
            return await _context.GroupTypes.FirstOrDefaultAsync(u => u.Name == name);
        }

        public void LoadParent(GroupType type)
        {
            _context.GroupTypes.Where(u => u.ParentN.Id == type.Id).Load();
            if (type.ParentN != null)
            {
                LoadParent(type.ParentN);
            }
        }

        public async Task<IEnumerable<GroupType>> LoadValidated()
        {
            return await _context.GroupTypes.Where(u => u.IsDeleted == false).ToListAsync();
        }

        public void RemoveAllChildren(Guid typeid)
        {
            var type = _context.GroupTypes.Find(typeid);
            var child = _context.GroupTypes.FirstOrDefault(u => u.ParentN != null & type.Id == u.ParentN.Id);

            if (child == null)
            {
                _context.RemoveRange(type);
                return;
            }
            RemoveAllChildren(child.Id);
        }

        public void RemoveRelation(GroupType type)
        {
            var t = _context.GroupTypes.Find(type);
            t.ParentN = null;
            _context.SaveChanges();
        }
        public async Task<GroupType> GetParents(Guid postId)
        {
            var child = await _context.GroupTypes.SingleOrDefaultAsync(u => u.Id == postId);

            if (child.ParentN == null)
                return child;
            return await GetParents(child.ParentN.Id);
        }
    }
}
