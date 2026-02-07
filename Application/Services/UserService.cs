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

        public async Task<PagedResult<AuthUser>> Search(int pageIndex, int pageSize, SortOption<SortUser> sortOption, string? search)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            return await _authUserRepository.GetAll(skip, pageSize, sortOption, search);
        }

        public async Task<GuestUser> CreateDefaultUser()
        {
            Random random = new Random();
            var randomNumber = random.Next(1000, 9999);

            var pseudo = $"User{randomNumber}";
            var user = new GuestUser(pseudo, "cat.jpg");

            return await _userRepository.InsertAsync(user);
        }
    }
}
