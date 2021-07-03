using ProjectManagerAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IReportRepository : IRepository<Report>
    {
        Task<IList<Report>> FindReportByGroupId(Guid groupId);
        Task<IList<Report>> FindReportByGroupIdAndProjectId(Guid groupId, Guid prjId);
        Task<IList<Report>> LoadValidated();
    }
}
