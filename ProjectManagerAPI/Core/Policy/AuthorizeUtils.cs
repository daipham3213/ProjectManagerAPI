using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Policy
{
    public class AuthorizeUtils
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorizeUtils(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Group>> GetValidatedGroups(AuthorizationHandlerContext context)
        {
            var username = context.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name)?.Value;
            var user = await this._unitOfWork.Users.GetUser(username);
            var leader = user.ParentN?.Id ?? user.Id;
            var groups = await this._unitOfWork.Groups.GetGroupListValidated(leader);
            return groups;
        }

        public async Task<IEnumerable<Group>> GetValidatedReport(AuthorizationHandlerContext context)
        {
            var groups = await GetValidatedGroups(context);

            return groups;
        }

        public async Task<bool> IsLeader(string username)
        {
            if (username == null)
                return false;
            var user = await this._unitOfWork.Users.GetUser(username);
            var group = await this._unitOfWork.Groups.GetGroupByLeaderId(user.Id);
            return group != null;
        }
    }

}
