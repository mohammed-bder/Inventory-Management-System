using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class EditViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Display(Name = "Last Name")]
        public string LName { get; set; }

        public string Phone { get; set; }
        public int? Salary { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Role { get; set; }
        public List<SelectListItem>? Roles { get; set; }

        // Add a property for EmployeeId to use for editing existing employees
        public int EmployeeId { get; set; }
    }
}
