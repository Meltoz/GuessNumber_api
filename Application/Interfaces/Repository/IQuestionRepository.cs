using Domain.Party;
using Shared;
using Shared.Filters;

namespace Application.Interfaces.Repository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        public Task<PagedResult<Question>> SearchQuestion(int skip, int take, QuestionFilter filterOption);
    }
}
