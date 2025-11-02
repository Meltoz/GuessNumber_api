using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public class ReportEntity : BaseEntity
    {
        [MaxLength(500)]
        public string Explanation { get; set; } = string.Empty;

        public string? Mail { get; set; }

        public TypeReport Type { get; set; }

        public ContextReport Context { get; set; }
    }
}
