using Application.Interfaces.Repository;
using Domain;
using Shared;

namespace Application.Services
{
    public class ReportService(IReportRepository rr)
    {
        private readonly IReportRepository _reportRepository = rr;

        public async Task<PagedResult<Report>> GetAll(int pageIndex, int pageSize, string? search)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            return await _reportRepository.Search(skip, pageSize, search);
        }
    }
}
