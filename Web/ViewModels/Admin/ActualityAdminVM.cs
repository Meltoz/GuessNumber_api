using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Admin
{
    public class ActualityAdminVM
    {
        public Guid? Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string StartDate { get; set; } = string.Empty;

        public string? EndDate { get; set; }

        public bool? Active { get; set; }
    }
}
