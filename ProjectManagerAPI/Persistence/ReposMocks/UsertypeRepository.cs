using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<UserType> GetTypeByName(string name)
        {
            return await this._context.UserTypes.FirstOrDefaultAsync(u => u.Name == name);
        }

        public void LoadParent(UserType type)
        {
            this._context.GroupTypes.Where(u => u.ParentN.ID == type.ID).Load();
            if (type.ParentN != null)
            {
                LoadParent(type.ParentN);
            }
        }

        public async Task<IEnumerable<UserType>> LoadValidated()
        {
            return await this._context.UserTypes.Where(u => u.IsDeleted == false).Distinct().ToListAsync();
        }

        public void RemoveAllChild(Guid id)
        {
            var type = this._context.UserTypes.Find(id);
            var child = this._context.UserTypes.FirstOrDefault(u => u.ParentN != null & type.ID == u.ParentN.ID);
            if (child == null)
            {
                this._context.RemoveRange(type);
                return;
            }
            RemoveAllChild(child.ID);
        }

        public void RemoveRelation(UserType type)
        {
            throw new NotImplementedException();
        }
    }
}
