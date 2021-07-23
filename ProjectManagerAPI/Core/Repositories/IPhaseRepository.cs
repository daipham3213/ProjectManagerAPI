using ProjectManagerAPI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IPhaseRepository : IRepository<Phase>
    {
        public Task<Phase> SearchPhaseByName(string name);
        void DeletePhase(Phase phase);
    }
}
