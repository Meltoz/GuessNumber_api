using Application.Interfaces.Repository;
using Domain.Enums;
using Domain.Party;

namespace Application.Services
{
    public class QuestionService(IQuestionRepository qr)
    {
        private readonly IQuestionRepository _questionRepository = qr;

        public async Task<Question> AddQuestion(string libelle, string response, Category category, VisibilityQuestion visibility, TypeQuestion type, string? author, string? unit)
        {
            var question = new Question(libelle, response, category, visibility, type, author, unit);

            var questionAdded = await _questionRepository.InsertAsync(question);

            return questionAdded;
        }
    }
}
