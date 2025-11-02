using Application.Interfaces.Repository;
using AutoMapper;
using Domain;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Infrastructure.Repositories
{
    public class ReportRepository(GuessNumberContext context, IMapper mapper) : BaseRepository<Report, ReportEntity>(context, mapper), IReportRepository
    {
        public async Task<PagedResult<Report>> Search(int skip, int take, string? search)
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(r => r.Explanation.ToLower().Contains(searchLower) || r.Mail.ToLower().Contains(searchLower));
            }
            query.OrderByDescending(r => r.Created);

            return await GetPaginateAsync(query, skip, take);
        }
    }
}
