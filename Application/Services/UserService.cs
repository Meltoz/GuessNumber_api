using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain.Enums;
using Domain.User;
using Domain.ValueObjects;
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

        public async Task<AuthUser> ChangeRole(Guid id, RoleUser newRole)
        {
            var authUser = await _authUserRepository.GetByIdAsync(id);

            if (authUser is null)
                throw new EntityNotFoundException(id);

            authUser.ChangeRole(newRole);

            return await _authUserRepository.UpdateAsync(authUser);
        }

        public async Task<AuthUser> ResetPassword(Guid id, string newPassword)
        {
            var authUser = await _authUserRepository.GetByIdAsync(id);

            if(authUser is null)
                throw new EntityNotFoundException(id);

            authUser.ChangePassword(newPassword);
            authUser.ChangePasswordNextTime();

            return await _authUserRepository.UpdateAsync(authUser);
        }

        public async Task<AuthUser> CreateAuthUser(string pseudo, string mail, string password)
        {
            var user = new AuthUser(pseudo, "cat.png", mail, password, RoleUser.User);

            return await _authUserRepository.InsertAsync(user);
        }

        public async Task<bool> IsMailAvailable(string mailtoCheck)
        {
            var mail = Mail.Create(mailtoCheck);
            return await _authUserRepository.CheckAvailableMail(mail);
        }

        public async Task<bool> IsPseudoAvailable(string pseudoToCheck)
        {
            var pseudo = Pseudo.Create(pseudoToCheck);
            return await _authUserRepository.CheckAvailablePseudo(pseudo);
        }

        public async Task<AuthUser> ConnectUser(string pseudo, string password)
        {
            var hashedPassword = Password.Create(password);
            var pseudoUser = Pseudo.Create(pseudo);

            var user =  await _authUserRepository.ConnectUser(pseudoUser, hashedPassword);

            if (user is null)
                throw new EntityNotFoundException();

            user.Login();
            await _authUserRepository.UpdateAsync(user);

            return user;
        }
    }
}
