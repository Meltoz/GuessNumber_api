using Domain;
using Shared;

namespace Application.Interfaces.Repository
{
    public interface IReportRepository : IRepository<Report>
    {
        public Task<PagedResult<Report>> Search(int skip, int take, string? search);
    }
}
