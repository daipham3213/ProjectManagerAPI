using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Resources;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ProjectManagerDbContext _context;
        public ProjectRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Project> SearchProjectByName(string name)
        {
            var project = await this._context.Projects.FirstOrDefaultAsync(u => u.Name == name);
            return project;
        }

        public async Task<IEnumerable<Project>> LoadValidated()
        {
            return await this._context.Projects.Where(u => u.IsDeleted == false).ToListAsync();
        }


        // BUG 
        public async void RemoveAllChildren(Guid ProjectId)
        {
            var project2 = await Get(ProjectId);
            var project = await _context.Reports.FindAsync(ProjectId);
            var child = _context.Projects.FirstOrDefault(u => u.Reports != null );

            if (child == null)
            {
                project2.Reports.Remove(project);
                _context.RemoveRange(project);
                return;
            }
            RemoveAllChildren(child.Id);
        }

        public async Task<bool> UpdateProject(Guid id,CreateProject project)
        {
            var result = await this._context.Projects.FindAsync(id);

            result.DueDate = project.DueDate;
            result.StartDate = project.StartDate;
            result.Name = project.Name;
            result.Remark = project.Remark;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
