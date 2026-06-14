using Domain.Party;
using Shared;
using Shared.Filters;

namespace Application.Interfaces.Repository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        public Task<PagedResult<Question>> SearchQuestion(int skip, int take, QuestionFilter filterOption);
        
        public Task<Dictionary<Guid, HashSet<Guid>>> GetAllQuestionIds(HashSet<Guid> categoriesIds);
        
        public Task<IEnumerable<Question>> GetQuestionsByIds(IEnumerable<Guid> questionIds);
    }
}
