using Domain.User;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Interfaces.Repository
{
    public interface IAuthUserRepository : IRepository<AuthUser>
    {
        public Task<PagedResult<User>> GetAll(int skip, int take, SortOption<SortUser> sortOption, string search, bool includeGuest = false);

        public Task<bool> CheckAvailablePseudo(string pseudo);

        public Task<bool> CheckAvailableMail(string mail);
    }
}
