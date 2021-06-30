using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Persistence;
using ProjectManagerAPI.Persistence.ReposMocks;

namespace ProjectManagerAPI.Core.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ProjectManagerDbContext _context;
        public ProjectRepository(ProjectManagerDbContext context)
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
