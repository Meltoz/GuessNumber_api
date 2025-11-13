using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Admin
{
    public class CategoryAdminVM : CategoryAdminAddVM
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class CategoryAdminAddVM
    {
        [MaxLength(500)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
    }
}
