using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        public void LoadParent(Project project);
        public void RemoveRelation(Project project);
    }
}
