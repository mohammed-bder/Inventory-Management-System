namespace Inventory_Management_System.ViewModel
{
    public class TransactionWithProducts
    {
        public int EmployeeId { get; set; }

        public List<ProductDetailsForTransaction> ProductDetails { get; set; }
        public List<Product>? AvailableProducts { get; set; }
        public List<Product>? SelectedProducts { get; set; }
        public double? TotalPrice { get; set; }

    }
}
