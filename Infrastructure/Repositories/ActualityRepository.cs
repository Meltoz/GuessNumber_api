using Application.Interfaces.Repository;
using AutoMapper;
using Domain;
using Infrastructure.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Enums.Sorting;

namespace Infrastructure.Repositories
{
    public class ActualityRepository : BaseRepository<Actuality, ActualityEntity>, IActualityRepository
    {
        public ActualityRepository(GuessNumberContext context, IMapper mapper)
        : base(context, mapper) { }

        public async Task<PagedResult<Actuality>> Search(int skip, int take, SortOption<SortActuality> sortOptions, string search = "")
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Title.Contains(search) || a.Content.Contains(search));
            }

            query = query.ApplySort(sortOptions);

            return await GetPaginateAsync(query, skip, take);
        }
    }
}
