using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class QuestionEntity : BaseEntity
    {
        [Required]
        public string Libelle { get; set; }

        [Required]
        public string Response { get; set; }

        public Guid CategoryId { get; set; }

        public CategoryEntity Category { get; set; }

        [MaxLength(100)]
        public string? Author { get; set; }

        public string? Unit { get; set; }

        public VisibilityQuestion Visibility { get; set; }

        public TypeQuestion Type { get; set; }
    }
}
