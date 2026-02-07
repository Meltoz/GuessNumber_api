using Application.Interfaces.Repository;
using AutoMapper;
using Domain.User;
using Infrastructure.Entities;

namespace Infrastructure.Repositories
{
    public class UserRepository(GuessNumberContext c, IMapper m) : BaseRepository<GuestUser, UserEntity>(c, m), IUserRepository
    {
    }
}
