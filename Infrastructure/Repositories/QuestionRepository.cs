using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Enums;
using Domain.Party;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Filters;

namespace Infrastructure.Repositories
{
    public class QuestionRepository(GuessNumberContext c, IMapper m) : BaseRepository<Question, QuestionEntity>(c, m), IQuestionRepository
    {
        public override async Task<Question> InsertAsync(Question domain)
        {
           var q =  await base.InsertAsync(domain);

            var questionInserted = await GetBaseQuery().AsNoTracking().SingleAsync(x => q.Id == x.Id);
            return _mapper.Map<Question>(questionInserted);

        }

        public async Task<PagedResult<Question>> SearchQuestion(int skip, int take, QuestionFilter filterOption)
        {
            var query = _dbSet
                .Include(q => q.Category)
                .AsNoTracking()
                .AsQueryable();

            query = FilterQuestion(query, filterOption);

            query = query.OrderByDescending(q => q.Created);

            return await GetPaginateAsync(query, skip, take);
        }

        public override async Task<Question?> GetByIdAsync(Guid id)
        {
            var q = await GetBaseQuery().AsNoTracking().SingleOrDefaultAsync(q => q.Id == id);

            return _mapper.Map<Question?>(q);
        }

        private IQueryable<QuestionEntity> FilterQuestion(IQueryable<QuestionEntity> query, QuestionFilter filterOption)
        {
            if(!string.IsNullOrWhiteSpace(filterOption.Libelle))
            {
                var searchedLibelle = filterOption.Libelle.ToLower();
                query = query.Where(q => q.Libelle.ToLower().Contains(searchedLibelle));
            }

            if (!string.IsNullOrWhiteSpace(filterOption.Author))
            {
                var searchAuthor = filterOption.Author.ToLower();
                query = query.Where(q => q.Author.ToLower().Contains(searchAuthor));
            }

            if (!string.IsNullOrWhiteSpace(filterOption.Categories))
            {
                var categories = filterOption.Categories
                   .Split(",", StringSplitOptions.RemoveEmptyEntries)
                   .Select(x => x.Trim())
                   .Where(x => !string.IsNullOrWhiteSpace(x) && Guid.TryParse(x, out _))
                   .Select(x => Guid.Parse(x))
                   .Distinct()
                   .ToList();

                if (categories.Any())
                {
                    query = query.Where(q => categories.Contains(q.CategoryId));
                }
            }

            if (filterOption.Visibility.HasValue)
            {
                var visibilityFilter = (VisibilityQuestion)filterOption.Visibility.Value;
                query = query.Where(q => (q.Visibility & visibilityFilter) != 0);
            }

            if (filterOption.Type.HasValue)
            {
                var typeFilter = (TypeQuestion)filterOption.Type.Value;
                query = query.Where(q => (q.Type & typeFilter) != 0);
            }

            return query;
        }

        protected virtual IQueryable<QuestionEntity> GetBaseQuery()
        {
            return _dbSet.Include(q => q.Category);
        }
    }
}
