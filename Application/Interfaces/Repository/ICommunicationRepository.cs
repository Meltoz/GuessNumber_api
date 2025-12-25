
using Domain;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Interfaces.Repository
{
    public interface ICommunicationRepository : IRepository<Communication>
    {
        public Task<PagedResult<Communication>> Search(int skip, int take, SortOption<SortCommunication> sortOption, string? search);

        public Task<IEnumerable<Communication>> GetActives();
    }
}
