using Application.Interfaces.Repository;
using Domain.Party;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Services
{
    public class CategoryService(ICategoryRepository cr)
    {
        private readonly ICategoryRepository _categoryRepository = cr;

        public async Task<PagedResult<Category>> Search(int pageIndex, int pageSize, SortOption<SortCategory> sortOption, string search = "")
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);
            var categories = await _categoryRepository.Search(skip, pageSize, sortOption, search);

            return categories;
        }

        public async Task<Category> CreateNew(string name)
        {
            var category = new Category(name);

            var categoryInserted = await _categoryRepository.InsertAsync(category);

            return categoryInserted;
        }
    }
}
