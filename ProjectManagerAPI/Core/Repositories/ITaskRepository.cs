using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface ITaskRepository : IRepository<Models.Task>
    {
        public Task<IEnumerable<Models.Task>> LoadByUser(User user);
        public Task<IEnumerable<Models.Task>> LoadByPhase(Phase phase );
        public Task<Report> LoadByReport(Report report);
        public Task<IList<Models.Task>> TaskSearchByPhaseId(Guid phaseId);
        public Task<Models.Task> SearchTaskByName(string nameTask);
        Task RemoveChild(Core.Models.Task parent);
        Task<float> GetContrib(Guid userId);
    }
}
