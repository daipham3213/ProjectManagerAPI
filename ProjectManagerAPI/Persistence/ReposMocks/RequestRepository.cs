using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.StaticValue;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        private readonly ProjectManagerDbContext _context;

        public RequestRepository(ProjectManagerDbContext context) : 
            base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Request>> GetRequestList(Guid userId)
        {
            var user = await this._context.Users.FindAsync(userId);
            var request = await this._context.Requests
                .Where(u => ((u.To == userId & !u.IsGroup)
                             | (u.To == user.GroupRef & u.IsGroup)) 
                            & !u.IsDeleted)
                .OrderByDescending(u => u.DateCreated)
                .ToListAsync();
            return request;
        }

        public async Task<IEnumerable<Request>> GetOldRequestList(Guid userId)
        {
            var result = await GetRequestList(userId);
            return result.Where(u => 
                (u.IsAccepted & !u.IsDenied) 
                | (!u.IsAccepted | u.IsDenied))
                .Take(10);
        }

        public async Task<IEnumerable<Request>> GetNewRequestList(Guid userId)
        {
            var result = await GetRequestList(userId);
            return result.Where(u => !u.IsDenied & !u.IsAccepted);
        }

        public async Task<int> CountNewRequest(Guid userId)
        {
            var requests = await GetNewRequestList(userId);
            return requests.Count();
        }

        public async Task<Request> CreateRequest(CreatedRequest request, User user)
        {
            var result = new Request()
            {
                Name = request.Name,
                Remark = request.Remark,
                UserCreated = user.Id,
                IsGroup = request.Name == RequestNames.CreateGroup,
                To = request.To,
                Value = request.Value
            };

            await this._context.Requests.AddAsync(result);
            await this._context.SaveChangesAsync();
            return result;
        }

        public async Task ProcessRequest(Guid id, bool isAccepted = false, bool isDenied = false)
        {
            var request = await this._context.Requests.FindAsync(id);
            request.IsDenied = isDenied;
            request.IsAccepted = isAccepted;
            request.DateModified = DateTime.UtcNow;
            await this._context.SaveChangesAsync();
        }
    }
}
