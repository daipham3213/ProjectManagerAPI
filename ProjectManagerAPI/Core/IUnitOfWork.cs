using ProjectManagerAPI.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IGroupTypeRepository GroupTypes { get; }
        IProjectRepository Projects { get; }
        IAvatarRepository Avatars { get; }
        IUserRepository Users { get; }
        IGroupRepository Groups { get; }
        IReportRepository Reports { get; }
        IPhaseRepository Phases { get;  }
        IRequestRepository Requests { get; }
        Task<int> Complete();
    }
}
