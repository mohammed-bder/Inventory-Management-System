using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Management_System.Models
{
    public enum TransactionType
    {
        Purchase,   // When a product is purchased or restocked
        Sale,       // When a product is sold or removed from inventory
        Return,     // When a product is returned to the inventory
        Adjustment  // When there is a manual adjustment (e.g., for errors, damage, etc.)
    }

    public class Transaction
    {

        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }


        //need to list of products details

        public int EmployeeId { get; set; }
        public Employee employee { get; set; }

        public List<ProductTransaction> ProductTransactions { get; set; }

    }
}
