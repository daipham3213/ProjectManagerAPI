using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        public Task<IEnumerable<Project>> LoadValidated();
        public Task<Project> SearchProjectByName(string name);
        public void RemoveAllChildren(Guid ReportId);
    }
}
