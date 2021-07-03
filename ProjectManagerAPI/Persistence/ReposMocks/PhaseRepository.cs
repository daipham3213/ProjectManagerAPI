using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class PhaseRepository : Repository<Phase>, IPhaseRepository {

        private readonly ProjectManagerDbContext _context;
        public PhaseRepository(ProjectManagerDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Phase> SearchPhaneByName(string name)
        {
            return await _context.Phases.FirstOrDefaultAsync(u => u.Name.ToLower().Trim().Contains(name.ToLower().Trim()));
        }

    
    }
}
