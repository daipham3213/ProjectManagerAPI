using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IReportRepository : IRepository<Report>
    {
        Task<IList<Report>> FindReportByGroupId(Guid groupId);
        Task<IList<Report>> FindReportByGroupIdAndProjectId(Guid groupId, Guid prjId);
    }
}
