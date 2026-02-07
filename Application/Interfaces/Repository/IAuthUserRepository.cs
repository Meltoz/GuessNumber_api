using Domain.User;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Interfaces.Repository
{
    public interface IAuthUserRepository : IRepository<AuthUser>
    {
        public Task<PagedResult<AuthUser>> GetAll(int skip, int take, SortOption<SortUser> sortOption, string search);
    }
}
