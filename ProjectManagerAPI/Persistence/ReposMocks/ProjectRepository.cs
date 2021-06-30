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
    }
}
