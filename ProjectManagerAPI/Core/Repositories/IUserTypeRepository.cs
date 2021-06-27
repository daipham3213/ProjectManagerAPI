using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IUserTypeRepository : IRepository<UserType>
    {
        public void LoadParent(UserType type);
        public void RemoveRelation(UserType type);
        public Task<IEnumerable<UserType>> LoadValidated();
        Task<UserType> GetTypeByName(string name);
        public void RemoveAllChild(Guid id);
    }
}
