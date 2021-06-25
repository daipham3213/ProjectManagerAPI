using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Repositories;

namespace ProjectManagerAPI.Core
{
    public interface IUnitOfWork: IDisposable
    {
        IGroupTypeRepository GroupType { get; }
        IProjectRepository Project { get; }

        Task<int> Complete();
    }
}
