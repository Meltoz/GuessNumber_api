using Domain.User;
using Domain.ValueObjects;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Interfaces.Repository
{
    public interface IAuthUserRepository : IRepository<AuthUser>
    {
        public Task<PagedResult<User>> GetAll(int skip, int take, SortOption<SortUser> sortOption, string search, bool includeGuest = false);

        public Task<bool> CheckAvailablePseudo(Pseudo pseudo);

        public Task<bool> CheckAvailableMail(Mail mail);

        public Task<AuthUser> ConnectUser(Pseudo pseudo, Password password);
    }
}
