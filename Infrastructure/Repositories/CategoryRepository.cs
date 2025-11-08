using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Party;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Enums.Sorting;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(GuessNumberContext context, IMapper mapper) : BaseRepository<Category, CategoryEntity>(context, mapper), ICategoryRepository
    {
        public Task<PagedResult<Category>> Search(int skip, int take, SortOption<SortCategory> sortOption, string search = "")
        {
            var query = _dbSet
                 .AsNoTracking()
                 .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(lowerSearch));
            }

            query = ApplySort(query, sortOption);


            return GetPaginateAsync(query, skip, take);
            
        }

        private static IQueryable<CategoryEntity> ApplySort(IQueryable<CategoryEntity> query, SortOption<SortCategory> sortOption) 
        {

            return (sortOption.SortBy, sortOption.Direction) switch
            {
                (SortCategory.Created, SortDirection.Ascending) => query.OrderBy(c => c.Created),
                (SortCategory.Created, SortDirection.Descending) => query.OrderByDescending(c => c.Created),
                (SortCategory.Name, SortDirection.Ascending) => query.OrderBy(c => c.Name),
                (SortCategory.Name, SortDirection.Descending) => query.OrderByDescending(c => c.Name),
                _ => query.OrderBy(c => c.Name)
            };
        }
    }
}
