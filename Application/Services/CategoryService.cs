using Application.Exceptions;
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

        public async Task<Category> CreateNewAsync(string name)
        {
            var category = new Category(name);

            var categoryInserted = await _categoryRepository.InsertAsync(category);

            return categoryInserted;
        }

        public async Task<Category> UpdateAsync(Category c) {
            if (c == null || c.Id == Guid.Empty)
                throw new ArgumentException("category");

            var category = await _categoryRepository.GetByIdAsync(c.Id);

            if (category is null)
                throw new EntityNotFoundException(c.Id);

            if(category.Name != c.Name)
            {
                category.ChangeName(c.Name);
            }

            var categoryUpdate = await _categoryRepository.UpdateAsync(category);

            return categoryUpdate;
        }


        public async Task DeleteAsync(Guid id)
        {
            var category = await GetByIdAsync(id);

            _categoryRepository.Delete(category.Id);
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            if(id == Guid.Empty)
                throw new ArgumentException("id");

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
                throw new EntityNotFoundException(id);

            return category;
        }
    }
}
