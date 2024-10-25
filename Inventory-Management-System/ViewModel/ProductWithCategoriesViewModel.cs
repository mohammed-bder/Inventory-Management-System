namespace Inventory_Management_System.ViewModel
{
    public class ProductWithCategoriesViewModel
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<Category>? categories { get; set; }
        public int SupplierId { get; set; }
        public List<Supplier>? suppliers { get; set; }


    }
}
