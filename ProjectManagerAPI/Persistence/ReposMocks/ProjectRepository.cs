using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Persistence;
using ProjectManagerAPI.Persistence.ReposMocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public class ProjectRepository : Persistence.ReposMocks.Repository<Project>, IProjectRepository
    {
        private readonly ProjectManagerDBContext _context;
        public ProjectRepository(ProjectManagerDBContext context)
            : base(context)
        {
            _context = context;
        }
   
        public async Task<Project> SearcProjectByName(string name)
        {
        
               var project = await this._context.Projects.FirstOrDefaultAsync(u => u.Name == name);
            return project;
        }

        public async Task<IEnumerable<Project>> LoadValidated()
        {
            return await this._context.Projects.Where(u => u.IsDeleted == false).ToListAsync();
        }
    }
}
