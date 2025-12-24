using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Admin
{
    public class ActualityAdminVM : ActualityVM
    {
        public Guid? Id { get; set; }

        public bool IsActive { get; set; }
    }
}
