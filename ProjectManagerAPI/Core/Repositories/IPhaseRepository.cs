using ProjectManagerAPI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Resources;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.Core.Repositories
{
    public interface IPhaseRepository : IRepository<Phase>
    {
        public Task<Phase> SearchPhaseByName(string name);
        void DeletePhase(Phase phase);
      
    }
}
