using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain;
using Domain.Enums;
using Shared;
using Shared.Enums;

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

        public async Task<Report> GetById(Guid id)
        {
            var report = await _reportRepository.GetByIdAsync(id);

            if (report is null)
                throw new EntityNotFoundException(id);

            return report;
        }

        public async Task Delete(Guid id)
        {
            var report = await _reportRepository.GetByIdAsync(id);

            if (report is null)
                throw new EntityNotFoundException(id);

            _reportRepository.Delete(report.Id);
        }

        public async Task<Report> CreateReport(string type, string context, string explanation, string? mail)
        {
            var successContext = EnumConverter.TryConvert(context, out ContextReport contextReport);
            if (!successContext)
                throw new ArgumentException("Error when converting context");

            var successType = EnumConverter.TryConvert(type, out TypeReport typeReport);

            if (!successType)
                throw new ArgumentException("Error when converting type");

            var report = new Report(typeReport, contextReport, explanation, mail);

            return await _reportRepository.InsertAsync(report);
        }
    }
}
