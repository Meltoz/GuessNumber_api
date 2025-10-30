using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
