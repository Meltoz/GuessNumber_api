using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain.Enums;
using Domain.Party;
using Shared;
using Shared.Filters;

namespace Application.Services
{
    public class QuestionService(IQuestionRepository qr)
    {
        private readonly IQuestionRepository _questionRepository = qr;

        public async Task<PagedResult<Question>> Search(int pageIndex, int pageSize, QuestionFilter filterOption)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);
            return await _questionRepository.SearchQuestion(skip, pageSize, filterOption);
        }

        public async Task<Question> GetById(Guid id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question is null)
                throw new EntityNotFoundException(id);

            return question;
        }

        public async Task<Question> AddQuestion(string libelle, string response, Category category, VisibilityQuestion visibility, TypeQuestion type, string? author, string? unit)
        {
            var question = new Question(libelle, response, category, visibility, type, author, unit);

            var questionAdded = await _questionRepository.InsertAsync(question);

            return questionAdded;
        }

        public async Task DeleteQuestion(Guid id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question is null)
                throw new EntityNotFoundException(id);

            _questionRepository.Delete(id);
        }
    }
}
