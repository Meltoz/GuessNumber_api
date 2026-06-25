using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Party;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class AnswerRepository(GuessNumberContext c, IMapper m) : BaseRepository<Answer, AnswerEntity>(c,m), IAnswerRepository
{
    
}