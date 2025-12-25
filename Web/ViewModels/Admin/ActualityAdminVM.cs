using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Admin
{
    public class ActualityAdminVM : ActualityVM
    {
        public Guid? Id { get; set; }

        [Required]
        public string StartDate { get; set; } = string.Empty;

        public string? EndDate { get; set; }

        public bool IsActive =>
            !string.IsNullOrEmpty(StartDate) &&
            DateTime.TryParse(StartDate, out var start) &&
            DateTime.UtcNow >= start &&
            (string.IsNullOrEmpty(EndDate) ||
             (DateTime.TryParse(EndDate, out var end) && DateTime.UtcNow <= end));
    }
}
