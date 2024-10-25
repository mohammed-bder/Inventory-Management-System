using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please select a Name.")]
        public string Name { get; set; }
        public string? Description { get; set; }


        public List<Product>? products { get; set; }
    }
}
