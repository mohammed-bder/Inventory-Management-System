namespace Inventory_Management_System.Repository
{
    public interface IProductTransactionRepository : IRepository<ProductTransaction>
    {
        public List<TopSellingProduct> GetTopSelling();

    }
}
