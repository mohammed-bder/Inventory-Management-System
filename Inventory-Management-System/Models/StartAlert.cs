namespace Inventory_Management_System.Models
{
    public class StartAlert
    {
        public int ID { get; set; }
        public DateTime AlertDate { get; set; }
        public int AlertQuantityLevel { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolveDate { get; set; }

        
        public int EmployeeId { get; set; }
        public Employee? employee { get; set; }


        public int ProductId { get; set; }
        public  Product? product { get; set; }
    }
}
