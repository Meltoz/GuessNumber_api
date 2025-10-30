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

        public bool? Active { get; set; }
    }
}
