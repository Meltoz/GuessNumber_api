using Domain;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Interfaces.Repository
{
    public interface IActualityRepository : IRepository<Actuality>
    {
        public Task<PagedResult<Actuality>> Search(int skip, int take, SortOption<SortActuality> searchOptions, string search = "");

        public Task<IEnumerable<Actuality>> GetActives();
    }
}
