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
    public class CommunicationRepository : BaseRepository<Communication, CommunicationEntity>, ICommunicationRepository
    {
        public CommunicationRepository(GuessNumberContext context, IMapper mapper) 
            : base(context, mapper) { }

        public async Task<PagedResult<Communication>> Search(int skip, int take, SortOption<SortCommunication> sortOption, string? search)
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(c => c.Content.ToLower().Contains(searchLower));
            }
            query = SortQuery(query, sortOption);

            return await GetPaginateAsync(query, skip, take);
        }

        public async Task<IEnumerable<Communication>> GetActives()
        {
            var today = DateTime.UtcNow;
            var communications = await _dbSet.Where(c => c.Start < today &&
            (!c.End.HasValue || c.End.Value > today))
                .ToListAsync();

            return _mapper.Map<IEnumerable<Communication>>(communications);
        }

        private IQueryable<CommunicationEntity> SortQuery (IQueryable<CommunicationEntity> query, SortOption<SortCommunication> sortOption)
        {
            if (query is null)
                return Enumerable.Empty<CommunicationEntity>().AsQueryable();

            var now = DateTime.UtcNow;

            return (sortOption.SortBy, sortOption.Direction) switch
            {
                (SortCommunication.Active, SortDirection.Ascending) =>
                    query.OrderBy(c => !(c.Start <= now && (c.End == null || c.End >= now))),
                (SortCommunication.Active, SortDirection.Descending) =>
                    query.OrderByDescending(c => !(c.Start <= now && (c.End == null || c.End >= now))),
                (SortCommunication.Created, SortDirection.Ascending) =>
                    query.OrderBy(c => c.Created),
                (SortCommunication.Created,SortDirection.Descending) =>
                    query.OrderByDescending(c => c.Created),

                _ => query
            };
        }
    }
}
