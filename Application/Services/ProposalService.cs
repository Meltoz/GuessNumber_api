using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain;

namespace Application.Services
{
    public class ProposalService(IProposalRepository pr)
    {
        private readonly IProposalRepository _proposalRepository = pr;

        public async Task<Proposal> GoToNext(Guid? idPrevious)
        {
            if(idPrevious != null)
            {
                await Delete(idPrevious.Value);
            }

            return await _proposalRepository.GetNext();
        }

        public async Task Delete(Guid id)
        {
            var proposal = await _proposalRepository.GetByIdAsync(id);

            if (proposal is null)
                throw new EntityNotFoundException(id);

            _proposalRepository.Delete(id);
        }
    }
}
