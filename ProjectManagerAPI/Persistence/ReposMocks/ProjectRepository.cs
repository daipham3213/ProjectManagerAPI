using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Persistence;
using ProjectManagerAPI.Persistence.ReposMocks;
using System;

namespace ProjectManagerAPI.Core.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ProjectManagerDBContext _context;
        public ProjectRepository(ProjectManagerDBContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
