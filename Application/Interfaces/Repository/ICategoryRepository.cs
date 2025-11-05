using Domain.Party;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Interfaces.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<PagedResult<Category>> Search(int skip, int take, SortOption<SortCategory> sortOption, string search = "");
    }
}
