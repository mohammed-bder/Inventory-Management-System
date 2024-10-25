namespace Inventory_Management_System.Repository
{
    public interface IEmployeeSupplierRepository : IRepository<EmployeeSupplier>
    {
        public void DeleteEmployeeSuppliers(List<int> employeeIds);
        public int GetOrdersCount();
        public EmployeeSupplier GetByDate(DateTime dateTime);
        public List<EmployeeSupplier> GetAllByProductId(int Id);
    }
}
