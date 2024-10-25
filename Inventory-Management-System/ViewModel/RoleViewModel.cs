using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class RoleViewModel
    {
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
