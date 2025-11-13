using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Party;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository(GuessNumberContext c, IMapper m) : BaseRepository<Question, QuestionEntity>(c, m), IQuestionRepository
    {
        public override async Task<Question> InsertAsync(Question domain)
        {
           var q =  await base.InsertAsync(domain);

            var questionInserted = await _dbSet.Include(c => c.Category).SingleAsync(x => q.Id == x.Id);
            return _mapper.Map<Question>(questionInserted);

        }
    }
}
