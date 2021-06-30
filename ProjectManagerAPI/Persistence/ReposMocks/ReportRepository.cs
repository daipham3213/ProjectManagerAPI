using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        private readonly ProjectManagerDbContext _context;
        public ReportRepository(ProjectManagerDbContext context) 
            : base(context)
        {
            _context = context;
        }
    }
}
