namespace Inventory_Management_System.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        public List<Product> GetFilteredByCategory(int? id);
        public List<Product> GetFilteredByName(string name);
        public List<Product> GetFilteredByNameWithCategory(string name, int? id);
        public List<Product> GetFilteredByStatus(string staus, int? id);
        public List<Product> GetByIds(List<int> ids);
        public List<Product> GetAllAvailable();
        public List<int>? GetLowQuantitesProducts();
        public int GetProductCount();
        public int GetItemsCount();
        public bool CheckExistence(string productName);
        public bool CheckExistenceNameForEdit(string productName);
        public Product GetByName(string name);
    }

}


