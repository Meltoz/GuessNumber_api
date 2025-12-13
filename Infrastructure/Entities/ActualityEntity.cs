using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class ActualityEntity : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime StartPublish { get; set; }

        public DateTime? EndPublish { get; set; }
    }
}
