using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ActualityVM
    {

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string StartDate { get; set; } = string.Empty;

        public string? EndDate { get; set; }
    }
}
