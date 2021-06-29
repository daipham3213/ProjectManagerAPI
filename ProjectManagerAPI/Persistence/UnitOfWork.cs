using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Repositories;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectManagerDBContext _context;
        public IGroupTypeRepository GroupTypes { get; private set; }
        public IProjectRepository Projects { get; private set; }

        public IAvatarRepository Avatars { get; private set; }

        public IUserRepository Users { get; private set; }


        public IGroupRepository Groups { get; private set; }

        public UnitOfWork(
            ProjectManagerDBContext context,
            IGroupTypeRepository groupTypes,
            IProjectRepository projects,
            IAvatarRepository avatars,
            IUserRepository users,        
            IGroupRepository groups
            )
        {
            _context = context;
            GroupTypes = groupTypes;
            Projects = projects;
            Avatars = avatars;
            Users = users;
            Groups = groups;
        }
        public async Task<int> Complete()
        {
            return await this._context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._context.Dispose();
        }
    }
}
