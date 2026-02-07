using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain.User;
using Shared;
using Shared.Enums.Sorting;

namespace Application.Services
{
    public class UserService(IUserRepository ur, IAuthUserRepository iaur)
    {
        private readonly IUserRepository _userRepository = ur;
        private readonly IAuthUserRepository _authUserRepository = iaur;

        public async Task<PagedResult<User>> Search(int pageIndex, int pageSize, SortOption<SortUser> sortOption, string? search, bool includeGuest = false)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            return await _authUserRepository.GetAll(skip, pageSize, sortOption, search, includeGuest);
        }

        public async Task<GuestUser> CreateDefaultUser()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000, 9999);

            var pseudo = $"User{randomNumber}";
            var user = new GuestUser(pseudo, "cat.jpg");

            return await _userRepository.InsertAsync(user);
        }

        public async Task<User> GetDetail(Guid id)
        {
            var authUser = await _authUserRepository.GetByIdAsync(id);
            if (authUser is not null)
                return authUser;

            var guestUser = await _userRepository.GetByIdAsync(id);
            if (guestUser is not null)
                return guestUser;

            throw new EntityNotFoundException(id);
        }
    }
}
