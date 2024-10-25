namespace Inventory_Management_System.Repository
{
    public interface IRepository<T>
    {
        public void Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public List<T> GetAll();
        public T GetById(int id);
        public void Save();

    }
}
