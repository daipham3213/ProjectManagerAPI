using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Repositories;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectManagerDbContext _context;
        public IGroupTypeRepository GroupTypes { get; private set; }
        public IProjectRepository Projects { get; private set; }

        public IAvatarRepository Avatars { get; private set; }

        public IUserRepository Users { get; private set; }

        public IRequestRepository Requests { get; private set; }
        public IGroupRepository Groups { get; private set; }
        public IReportRepository Reports { get; private set; }

        public IPhaseRepository Phases { get; private set; }
        public ITaskRepository Tasks { get; private set; }
        public IRefreshToken RefreshToken { get; private set; }

        public UnitOfWork(
            ProjectManagerDbContext context,
            IGroupTypeRepository groupTypes,
            IProjectRepository projects,
            IAvatarRepository avatars,
            IUserRepository users,
            IGroupRepository groups,
            IReportRepository reports,
            IPhaseRepository phases,
            IRequestRepository requests,
            ITaskRepository tasks,
            IRefreshToken refreshToken
        )
        {
            _context = context;
            GroupTypes = groupTypes;
            Projects = projects;
            Avatars = avatars;
            Users = users;
            Groups = groups;
            Reports = reports;
            Phases = phases;
            Requests = requests;
            Tasks = tasks;
            RefreshToken = refreshToken;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}