using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class EmployeeSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary key

        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public virtual Employee? Employee { get; set; }

        [Required(ErrorMessage = "Please select a Supplier.")]
        public int SupplierID { get; set; }
        [ForeignKey("SupplierID")]
        public virtual Supplier? Supplier { get; set; }

        // Additional attributes
        [Required(ErrorMessage = "Please select a Product.")]
        public int ProductIdentifier { get; set; }
        public DateTime StartDate { get; set; }
        [Display(Name = "Total Cost")]
        [Required(ErrorMessage = "The Total Cost field is required")]
        public double TotalCost { get; set; }
        [Required(ErrorMessage = "The Quantity field is required")]
        public int Quantity { get; set; }
    }

}
