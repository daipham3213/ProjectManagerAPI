using System;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Repositories;

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
        Task<int> Complete();
    }
}
