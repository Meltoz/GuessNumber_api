using Application.Interfaces.Repository;
using AutoMapper;
using Domain.User;
using Infrastructure.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Enums.Sorting;

namespace Infrastructure.Repositories
{
    public class AuthUserRepository(GuessNumberContext c, IMapper m) : BaseRepository<AuthUser, AuthUserEntity>(c, m), IAuthUserRepository
    {
        public Task<PagedResult<AuthUser>> GetAll(int skip, int take, SortOption<SortUser> sortOption, string search)
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable();

            if(!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(au => au.Pseudo.Contains(search) || au.Email.Contains(search));
            }
            
            query = query.ApplySort(sortOption);

            return GetPaginateAsync(query, skip, take);
        }
    }
}
