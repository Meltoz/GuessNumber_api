using Application.Interfaces.Repository;
using Domain;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Services
{
    public class CommunicationService(ICommunicationRepository cr)
    {
        private readonly ICommunicationRepository _communicationRepository = cr;

        public async Task<PagedResult<Communication>> Search(int pageIndex, int pageSize, SortOption<SortCommunication> sortOption, string? search)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            return await _communicationRepository.Search(skip, pageSize, sortOption, search);
        }
    }
}
