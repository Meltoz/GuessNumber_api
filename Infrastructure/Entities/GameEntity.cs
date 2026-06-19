using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class GameEntity : BaseEntity
    {
        [MaxLength(10)]
        public string Code { get; set; }

        public GameStatus Status { get; set; }
        
        public GameType Type { get; set; }
        
        public GamePhase? Phase { get; set; }

        public int CurrentQuestion { get; set; }

        public int TotalQuestion { get; set; }

        public int MaxPlayers { get; set; }
        
        public DateTime? PhaseStartedAt { get; set; }
        
        public DateTime? PhaseEndedAt { get; set; }
        public ICollection<PlayerEntity> Players { get; set; }
        
        public ICollection<GameCategoriesEntity> Categories { get; set; }
        
        public ICollection<GameQuestionEntity> Questions { get; set; }
        
    }
}
