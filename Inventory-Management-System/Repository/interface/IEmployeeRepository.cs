namespace Inventory_Management_System.Repository
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        public List<Employee> GetByName(string name);
        public void DeleteEmployees(List<int> employeeIds);
        public int GetEmpCount();
        public int GetLastCreatedEmp();
    }
}
