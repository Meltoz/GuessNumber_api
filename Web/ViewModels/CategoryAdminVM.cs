using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class CategoryAdminVM
    {
        public Guid? Id { get; set; }

        [MaxLength(500)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
    }
}
