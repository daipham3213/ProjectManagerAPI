using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IList<Report>> FindReportByGroupId(Guid groupId)
        {
            return await this._context.Reports.Where(u => u.GroupId == groupId).ToListAsync();
        }

        public async Task<IList<Report>> FindReportByGroupIdAndProjectId(Guid groupId, Guid prjId)
        {
            return await this._context.Reports.Where(u => u.GroupId == groupId & u.ProjectId == prjId).ToListAsync();
        }

        public async Task<IList<Report>> LoadValidated()
        {


            return null;
        }
    }
}
