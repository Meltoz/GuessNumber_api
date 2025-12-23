using Application.Interfaces.Repository;
using AutoMapper;
using Domain;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProposalRepository : BaseRepository<Proposal, ProposalEntity>, IProposalRepository
    {
        public ProposalRepository(GuessNumberContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<Proposal> GetNext()
        {
            var proposal = await _dbSet
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return _mapper.Map<Proposal>(proposal);
        }

        public async Task<int> CountProposal()
        {
            return await _dbSet.CountAsync();
        }
    }
}
