using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Resources;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        public Task<IEnumerable<Project>> LoadValidated();
        public Task<Project> SearchProjectByName(string name);
        public void RemoveAllChildren(Guid ReportId);
        Task<bool> UpdateProject(Guid id,CreateProject project);
    }
}
