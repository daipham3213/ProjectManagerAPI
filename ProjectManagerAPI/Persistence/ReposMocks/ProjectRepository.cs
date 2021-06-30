using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class ProjectRepository :Repository<Project>, IProjectRepository
    {
        private readonly ProjectManagerDBContext _context;
        public ProjectRepository(ProjectManagerDBContext context)
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
    }
}
