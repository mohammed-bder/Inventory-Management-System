
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Repository.repo
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductTransactionRepository(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }
        public void Add(ProductTransaction entity)
        {
            _applicationDbContext.ProductTransactions.Add(entity);
        }

        public void Delete(ProductTransaction entity)
        {
            _applicationDbContext.Remove(entity);
        }

        public List<ProductTransaction> GetAll()
        {
            throw new NotImplementedException();
        }

        public ProductTransaction GetById(int id)
        {
            throw new NotImplementedException();
        }
        public List<int> GetByProductId(int id)
        {
            return _applicationDbContext.ProductTransactions.Where(p => p.ProductId == id).Select(p => p.Quantity).ToList();
        }
        //public List<TopSellingProduct> GetTopSelling()
        //{
        //    List<TopSellingProduct> topProducts = _applicationDbContext.ProductTransactions
        //        .Include(pt => pt.Product) 
        //        .GroupBy(pt => pt.ProductId)
        //        .Select(g => new TopSellingProduct
        //        {
        //            ProductId = g.Key,
        //            TotalQuantity = g.Sum(pt => pt.Quantity),
        //            ProductName = g.FirstOrDefault().Product.Name 
        //        })
        //        .OrderByDescending(g => g.TotalQuantity)
        //        .Take(6)
        //        .ToList();

        //    return topProducts;
        //}
        public List<TopSellingProduct> GetTopSelling()
        {
            var topProducts = _applicationDbContext.ProductTransactions
                .Include(pt => pt.Product)
                .GroupBy(pt => pt.ProductId)
                .Select(g => new TopSellingProduct
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(pt => pt.Quantity),
                    ProductName = g.Select(pt => pt.Product.Name).FirstOrDefault() // safer way to get the name
                })
                .OrderByDescending(g => g.TotalQuantity)
                
                .ToList();

            return topProducts;
        }

        public void Save()
        {
            _applicationDbContext.SaveChanges();
        }

        public void Update(ProductTransaction entity)
        {
            throw new NotImplementedException();
        }
    }
}
