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
        public async Task<PagedResult<User>> GetAll(int skip, int take, SortOption<SortUser> sortOption, string search, bool includeGuest = false)
        {
            if (!includeGuest)
            {
                var query = _dbSet
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(au => au.Pseudo.Contains(search) || au.Email.Contains(search));

                query = query.ApplySort(sortOption);

                var total = await query.CountAsync();
                var entities = await query.Skip(skip).Take(take).ToListAsync();

                return new PagedResult<User>
                {
                    Data = _mapper.Map<IEnumerable<AuthUser>>(entities),
                    TotalCount = total
                };
            }

            var allQuery = _context.Set<UserEntity>()
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                allQuery = allQuery.Where(u => u.Pseudo.Contains(search)
                    || (u is AuthUserEntity && ((AuthUserEntity)u).Email.Contains(search)));

            allQuery = ApplySortToUsers(allQuery, sortOption);

            var totalCount = await allQuery.CountAsync();
            var allEntities = await allQuery.Skip(skip).Take(take).ToListAsync();

            var users = allEntities.Select<UserEntity, User>(e => e is AuthUserEntity auth
                ? _mapper.Map<AuthUser>(auth)
                : _mapper.Map<GuestUser>(e));

            return new PagedResult<User>
            {
                Data = users,
                TotalCount = totalCount
            };
        }

        private static IQueryable<UserEntity> ApplySortToUsers(IQueryable<UserEntity> query, SortOption<SortUser> sortOption)
        {
            return sortOption.SortBy switch
            {
                SortUser.Pseudo => sortOption.Direction == SortDirection.Ascending
                    ? query.OrderBy(u => u.Pseudo)
                    : query.OrderByDescending(u => u.Pseudo),
                SortUser.Created => sortOption.Direction == SortDirection.Ascending
                    ? query.OrderBy(u => u.Created)
                    : query.OrderByDescending(u => u.Created),
                SortUser.LastLogin => sortOption.Direction == SortDirection.Ascending
                    ? query.OrderBy(u => ((AuthUserEntity)u).LastLogin)
                    : query.OrderByDescending(u => ((AuthUserEntity)u).LastLogin),
                _ => query
            };
        }
    }
}
