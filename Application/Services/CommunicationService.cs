using Application.Exceptions;
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

        public async Task<Communication> CreateNew(string content, DateTime start, DateTime? end)
        {
            var communication = new Communication(content, start, end);

            return await _communicationRepository.InsertAsync(communication);
        }

        public async Task<Communication> UpdateAsync(Communication communication)
        {
            if(communication is null || communication.Id == Guid.Empty)
                throw new ArgumentNullException(nameof(communication));
            
            var commToUpdate = await _communicationRepository.GetByIdAsync(communication.Id);

            if (commToUpdate is null)
                throw new EntityNotFoundException(communication.Id);

            if (commToUpdate.Content != communication.Content)
                commToUpdate.ChangeContent(communication.Content);

            if (commToUpdate.StartDate != communication.StartDate
             || commToUpdate.EndDate != communication.EndDate)
                commToUpdate.ChangeDates(communication.StartDate, communication.EndDate);

            return await _communicationRepository.UpdateAsync(commToUpdate);
        }
            
        public async Task DeleteAsync(Guid idActuality)
        {
            var communication = await _communicationRepository.GetByIdAsync(idActuality);

            if (communication is null)
                throw new EntityNotFoundException(idActuality);

            _communicationRepository.Delete(idActuality);
        }

        public async Task<IEnumerable<Communication>> GetActiveCommunications()
        {
            var communications = await _communicationRepository.GetActives();

            return communications;
        }
    }
}
