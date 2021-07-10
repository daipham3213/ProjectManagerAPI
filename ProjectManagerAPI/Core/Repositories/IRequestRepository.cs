using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<IEnumerable<Request>> GetRequestList(Guid userId);
        Task<IEnumerable<Request>> GetOldRequestList(Guid userId);
        Task<IEnumerable<Request>> GetNewRequestList(Guid userId);
        Task<IEnumerable<Request>> GetSentRequestList(Guid userId);
        Task<int> CountNewRequest(Guid userId);
        Task<Request> CreateRequest(CreatedRequest request, User user);
        Task ProcessRequest(Guid id, bool isAccepted = false, bool isDenied = false);
    }
}
