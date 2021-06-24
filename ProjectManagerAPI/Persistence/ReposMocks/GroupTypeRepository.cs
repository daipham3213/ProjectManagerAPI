using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Linq;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class GroupTypeRepository : Repository<GroupType>, IGroupTypeRepository
    {
        private readonly ProjectManagerDBContext _context;
        public GroupTypeRepository (ProjectManagerDBContext context)
            :base (context)
        {
            _context = context;
        }
        public void LoadParent(GroupType type)
        {
            this._context.GroupTypes.Where(u => u.ParentN == type).Load();
            if (type.ParentN == null) return;

            LoadParent(type);
        }

        public void RemoveRelation(GroupType type)
        {
            var t = this._context.GroupTypes.Find(type);
            t.ParentN = null;
            _context.SaveChanges();
        }
    }
}
