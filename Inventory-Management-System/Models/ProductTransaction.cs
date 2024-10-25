namespace Inventory_Management_System.Models
{
    public class ProductTransaction
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public int Quantity { get; set; }
    }
}
