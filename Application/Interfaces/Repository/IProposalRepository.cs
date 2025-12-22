using Domain;

namespace Application.Interfaces.Repository
{
    public interface IProposalRepository : IRepository<Proposal>
    {
        public Task<Proposal> GetNext();
    }
}
