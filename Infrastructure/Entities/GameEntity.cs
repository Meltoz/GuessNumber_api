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

        public int CurrentQuestion { get; set; }

        public int TotalQuestion { get; set; }

        public int MaxPlayers { get; set; }
        
        public ICollection<PlayerEntity> Players { get; set; }
    }
}
