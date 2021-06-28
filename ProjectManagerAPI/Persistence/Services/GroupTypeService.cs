using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.Services
{
    public class GroupTypeService : IGroupTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupTypeService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<GroupType> GetParents(Guid postId)
        {
            var child = await _unitOfWork.GroupTypes.SingleOrDefault(u => u.ID == postId);

            if (child.ParentN == null)
                return child;
            return await GetParents(child.ParentN.ID);
        }
    }
}
