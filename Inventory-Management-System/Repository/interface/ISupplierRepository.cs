namespace Inventory_Management_System.Repository
{
    public interface ISupplierRepository:IRepository<Supplier>
    {
        List<Supplier> SearchByName(string name);
        public int GetSupplierCount();
    }
}
