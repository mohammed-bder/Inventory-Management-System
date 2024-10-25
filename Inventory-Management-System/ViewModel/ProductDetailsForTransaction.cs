using Inventory_Management_System.CustomAttributes;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class ProductDetailsForTransaction
    {
        [Required(ErrorMessage = "This field is required.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [QuantityAvailable(nameof(ProductId), ErrorMessage = "Quantity exceeds available stock.")]
        public int Quantity { get; set; }
        public int? UnitPrice { get; set; }
    }
}
