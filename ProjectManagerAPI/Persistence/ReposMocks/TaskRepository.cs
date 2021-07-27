using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using Task = ProjectManagerAPI.Core.Models.Task;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        private readonly ProjectManagerDbContext _context;

        public TaskRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<Core.Models.Task> SearchTaskByName(string nameTask)
        {
            var task = await _context.Tasks.SingleOrDefaultAsync(u => u.Name == nameTask);
            return task;
        }

        public async System.Threading.Tasks.Task RemoveChild(Task parent)
        {
            var childs = await _context.Tasks.Where(u => u.ParentN != null && u.ParentN == parent).ToListAsync();
            if (childs == null)
            {
                 _context.Tasks.Remove(parent);
                 await _context.SaveChangesAsync();
            }
            foreach (var child in childs)
            {
                await RemoveChild(child);
            }
        }

        public async Task<float> GetContrib(Guid userId)
        {
            var list = await this._context.Tasks.Where(u => u.UserId == userId).ToListAsync();
            var contrib = list.Sum(u => u.Percent) / (list.Count != 0 ? list.Count : 1);
            return contrib;
        }

        public async Task<IEnumerable<Task>> LoadByUser(User user)
        {
            return await this._context.Tasks.Where(u => u.IsDeleted == false && u.UserId == user.Id).ToListAsync();
        }

        public async Task<IEnumerable<Task>> LoadByPhase(Phase phase)
        {
            return await this._context.Tasks.Where(u => u.IsDeleted == false && u.PhaseId == phase.Id).ToListAsync();
        }

        public async Task<Report> LoadByReport(Report report)
        {
            foreach (var phase in report.Phases)
            {
                await this._context.Tasks.Where(u => u.PhaseId == phase.Id).LoadAsync();
            }
            return report;
        }

        public async Task<IList<Core.Models.Task>> TaskSearchByPhaseId(Guid phaseId)
        {
            return await this._context.Tasks.Where(u => u.PhaseId == phaseId).ToListAsync();
        }

        public async Task<int> UpdateProgress(Guid phaseId)
        {
            var phase = await this._context.Phases.FindAsync(phaseId);
            var report = await this._context.Reports.FindAsync(phase.ReportId);
            var totalTask = 0;
            float totalPercent = 0;
            foreach (var phaseReport in report.Phases)
            {
                totalTask += phaseReport.Tasks.Count;
                totalPercent += phaseReport.Tasks.Sum(taskPhase => taskPhase.Percent);
            }
            report.Progress = (totalPercent / (totalTask == 0 ? 1 : totalTask));
            return 1;
        }
    }
}