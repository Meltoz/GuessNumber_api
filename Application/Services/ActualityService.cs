using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Services
{
    public class ActualityService(IActualityRepository ar)
    {
        private readonly IActualityRepository _actualityRepository = ar;

        public async Task<PagedResult<Actuality>> Search(int pageIndex, int pageSize, SortOption<SortActuality> options, string search)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            return await _actualityRepository.Search(skip, pageSize, options, search);
        }

        public async Task<Actuality> CreateNew(string title, string content, DateTime start, DateTime? end)
        {
            var actuality = new Actuality(title, content, start, end);

            return await _actualityRepository.InsertAsync(actuality);
        }
        
        public async Task<Actuality> UpdateAsync(Actuality actuality)
        {
            var actualitytoUpdate = await _actualityRepository.GetByIdAsync(actuality.Id);

            if (actualitytoUpdate is null)
                throw new EntityNotFoundException(actuality.Id);

            if (actualitytoUpdate.Title != actuality.Title)
                actualitytoUpdate.ChangeTitle(actuality.Title);

            if (actualitytoUpdate.Content != actuality.Content)
                actualitytoUpdate.ChangeContent(actuality.Content);

            if (actualitytoUpdate.StartPublish != actuality.StartPublish
                || actualitytoUpdate.EndPublish != actuality.EndPublish)
                actualitytoUpdate.ChangePublish(actuality.StartPublish, actuality.EndPublish);

            return await _actualityRepository.UpdateAsync(actualitytoUpdate);
        }

        public async Task DeleteActualityAsync(Guid idActuality)
        {
            var actuality = await _actualityRepository.GetByIdAsync(idActuality);

            if (actuality is null)
                throw new EntityNotFoundException(idActuality);

            _actualityRepository.Delete(idActuality);

        }

        public async Task<IEnumerable<Actuality>> GetActiveActualities() 
        {
            return await _actualityRepository.GetActives();
        }
    }
}
