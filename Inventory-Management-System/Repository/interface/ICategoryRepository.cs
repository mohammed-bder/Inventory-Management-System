namespace Inventory_Management_System.Repository
{
    public interface ICategoryRepository:IRepository<Category>
    {
        public int GetAllCount();
        public void DeleteCategories(List<int> categoryIds);
        public List<Category> GetByName(string name);
    }
}
