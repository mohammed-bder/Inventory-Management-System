namespace Inventory_Management_System.Models
{
    public class Supplier
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<EmployeeSupplier>? employeeSuppliers { get; set; }
        public List<Product>? products { get; set; }


    }
}
