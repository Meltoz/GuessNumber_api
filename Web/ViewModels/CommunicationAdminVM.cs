using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class CommunicationAdminVM
    {
        public Guid? Id { get; set; }

        public string? StartDate { get; set; } = string.Empty;

        public string? EndDate { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool Active =>
                !string.IsNullOrEmpty(StartDate) &&
                DateTime.TryParse(StartDate, out var start) &&
                DateTime.UtcNow >= start &&
                (string.IsNullOrEmpty(EndDate) ||
                 (DateTime.TryParse(EndDate, out var end) && DateTime.UtcNow <= end));
    }
}
