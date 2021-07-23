using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = ProjectManagerAPI.Core.Models.Task;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class PhaseRepository : Repository<Phase>, IPhaseRepository {

        private readonly ProjectManagerDbContext _context;
        public PhaseRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Phase> SearchPhaseByName(string name)
        {
            return await _context.Phases.FirstOrDefaultAsync(u => u.Name.ToLower().Trim().Contains(name.ToLower().Trim()));
        }

        public void DeletePhase(Phase phase)
        {
            _context.Phases.Remove(phase);
            _context.SaveChanges();
        }
        
    }
}
