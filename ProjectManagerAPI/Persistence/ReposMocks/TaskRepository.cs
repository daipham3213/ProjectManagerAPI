using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class TaskRepository : Repository<Core.Models.Task>, ITaskRepository
    {

        private readonly ProjectManagerDbContext _context;
        public TaskRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Core.Models.Task>> LoadValidated()
        {
            return await this._context.Tasks.Where(u => u.IsDeleted == false).ToListAsync();
        }

        public async Task<Core.Models.Task> SearchTaskByName(string nameTask)
        {
            var task = await _context.Tasks.SingleOrDefaultAsync(u => u.Name == nameTask);
            return task;
        }

        public async Task<IList<Core.Models.Task>> TaskSearchByPhaseId(Guid phaseId)
        {
            return await this._context.Tasks.Where(u => u.PhaseId == phaseId).ToListAsync();
        }
    }
}
