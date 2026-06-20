using Domain.Party;

namespace Application.Results;

public record NextQuestionResult(bool IsGameOver, Question? Question)
{
    public static NextQuestionResult GameOver() => new (true, null );
    public static NextQuestionResult WithQuestion(Question q) => new(false, q);

}