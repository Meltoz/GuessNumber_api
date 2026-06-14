using Application.Exceptions;
using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Party;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GameRepository(GuessNumberContext c, IMapper m) : BaseRepository<Game, GameEntity>(c, m), IGameRepository
    {
        public async Task<Game> FindByCode(string code)
        {
            var game = await _dbSet
                .Include(g => g.Players)
                .Include(g => g.Categories).ThenInclude(gc => gc.Category)
                .Include(g => g.Questions).ThenInclude(gc => gc.Question)
                .Where(g => g.Code.ToLower() == code.ToLower())
                .SingleOrDefaultAsync();

            return _mapper.Map<Game>(game);
        }

        public async Task<Game?> FindByConnectionId(string connectionId)
        {
            var game = await _dbSet
                .Include(g => g.Players)
                .Include(g => g.Categories).ThenInclude(gc => gc.Category)
                .Include(g => g.Questions).ThenInclude(gc => gc.Question)
                .Where(g => g.Players.Any(p => p.ConnectionId == connectionId))
                .SingleOrDefaultAsync();

            return game is null ? null : _mapper.Map<Game>(game);
        }

        public override async Task<Game> UpdateAsync(Game domainToUpdate)
        {
            var existingEntity = await _dbSet
                .Include(g => g.Players)
                .SingleOrDefaultAsync(g => g.Id == domainToUpdate.Id);

            if (existingEntity is null)
                throw new EntityNotFoundException(domainToUpdate.Id);

            existingEntity.Code = domainToUpdate.Code;
            existingEntity.CurrentQuestion = domainToUpdate.CurrentQuestion;
            existingEntity.MaxPlayers = domainToUpdate.Settings.MaxPlayers;
            existingEntity.Status = domainToUpdate.Status;
            existingEntity.TotalQuestion = domainToUpdate.Settings.TotalQuestion;
            existingEntity.Type = domainToUpdate.Type;

            var domainPlayerIds = domainToUpdate.Players.Select(p => p.Id).ToHashSet();
            var existingPlayerIds = existingEntity.Players.Select(p => p.Id).ToHashSet();

            // Suppression des joueurs retirés
            var toRemove = existingEntity.Players.Where(p => !domainPlayerIds.Contains(p.Id)).ToList();
            foreach (var player in toRemove)
                _context.Remove(player);

            // Ajout des nouveaux joueurs (Id == Guid.Empty car créés côté domaine)
            foreach (var player in domainToUpdate.Players.Where(p => !existingPlayerIds.Contains(p.Id)))
            {
                existingEntity.Players.Add(new PlayerEntity
                {
                    GameId = existingEntity.Id,
                    UserId = player.UserId,
                    Pseudo = player.Pseudo,
                    Avatar = player.Avatar,
                    Score = player.Score,
                    Role = player.Role,
                    ConnectionId = player.ConnectionId
                });
            }

            // Mise à jour des joueurs existants
            foreach (var player in domainToUpdate.Players.Where(p => existingPlayerIds.Contains(p.Id)))
            {
                var existing = existingEntity.Players.First(ep => ep.Id == player.Id);
                existing.Score = player.Score;
                existing.Role = player.Role;
                existing.ConnectionId = player.ConnectionId;
            }

            await _context.Entry(existingEntity).Collection(g => g.Questions).LoadAsync();

            var domainQuestions = domainToUpdate.Questions.ToList();
            var domainQuestionIds = domainQuestions.Select(q => q.Id).ToHashSet();
            var existingQuestionIds = existingEntity.Questions.Select(q => q.QuestionId).ToHashSet();

            foreach (var q in existingEntity.Questions.Where(q => !domainQuestionIds.Contains(q.QuestionId)).ToList())
                _context.Remove(q);

            for (var i = 0; i < domainQuestions.Count; i++)
            {
                if (!existingQuestionIds.Contains(domainQuestions[i].Id))
                    existingEntity.Questions.Add(new GameQuestionEntity
                    {
                        GameId = existingEntity.Id,
                        QuestionId = domainQuestions[i].Id,
                        Order = i + 1
                    });
            }

            await SaveAsync();

            await _context.Entry(existingEntity)
                .Collection(g => g.Players)
                .LoadAsync();

            await _context.Entry(existingEntity)
                .Collection(g => g.Questions)
                .Query()
                .Include(gq => gq.Question)
                .LoadAsync();
            return _mapper.Map<Game>(existingEntity);
        }
    }
}
