using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain;

namespace Application.Services
{
    public class ProposalService(IProposalRepository pr)
    {
        private readonly IProposalRepository _proposalRepository = pr;

        public async void Delete(Guid id)
        {
            var proposal = _proposalRepository.GetByIdAsync(id);

            if (proposal is null)
                throw new EntityNotFoundException(id);

            _proposalRepository.Delete(id);
        }
    }
}
