using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Admin
{
    public class QuestionAdminVM : QuestionAdminAddVM
    {
        [Required]
        public Guid Id { get; set; }

    }

    public class QuestionAdminAddVM
    {
        [Required]
        public string Libelle { get; set; }

        [Required]
        public int Response { get; set; }

        public CategoryAdminVM Category { get; set; }

        public VisibilityQuestion Visibility { get; set; }

        public TypeQuestion Type { get; set; }

        public string? Author { get; set; }

        public string? Unit { get; set; }
    }
}
