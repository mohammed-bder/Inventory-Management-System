using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class EmployeeSupplierWithSupplierList
    {
        public int Id { get; set; } // Primary key
        public int EmployeeID { get; set; }
        public List<Employee>? Employees { get; set; }

        [Required(ErrorMessage = "Please select a Supplier.")]
        public int SupplierID { get; set; }
        public List<Supplier>? Suppliers { get; set; }

        // Additional attributes
        [Required(ErrorMessage = "Please select a Product.")]
        public int ProductIdentifier { get; set; }
        public List<Product>? Products { get; set; }
        public DateTime StartDate { get; set; }
        [Display(Name = "Total Cost")]
        [Required(ErrorMessage = "The Total Cost field is required")]
        public double TotalCost { get; set; }
        [Required(ErrorMessage = "The Quantity field is required")]
        public int Quantity { get; set; }
    }
}
