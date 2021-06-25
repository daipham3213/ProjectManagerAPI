using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using ProjectManagerAPI.Persistence;
using ProjectManagerAPI.Persistence.ReposMocks;
using System;
using System.Linq;

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


        public void RemoveRelation(Project project)
        {
            throw new NotImplementedException();
        }
    }
}
