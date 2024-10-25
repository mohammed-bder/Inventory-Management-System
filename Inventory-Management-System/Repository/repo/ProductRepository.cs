
using Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Repository.repo
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductRepository(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }

        public void Add(Product product)
        {
            _applicationDbContext.Products.Add(product);
        }
        public bool CheckExistence(string productName)
        {
            return _applicationDbContext.Products.Any(p => p.Name == productName);
        }
        public bool CheckExistenceNameForEdit(string productName)
        {
            return _applicationDbContext.Products.Where(p => p.Name == productName).Count()<=1;//< for edit name with valid name /*****/ = for edit another data without change name
        }
        public void Delete(Product product)
        {
            _applicationDbContext.Products.Remove(product);
        }

        public List<Product> GetAll()
        {
            return _applicationDbContext.Products.Include(p => p.category).Include(p => p.supplier).ToList();
        }
        public List<Product> GetAllAvailable()
        {
            return _applicationDbContext.Products.Where(p=>p.StockQuantity>0).ToList();
        }
        public List<Product> GetFilteredByCategory(int? id)
        {
            if (id == null)
                return GetAll();
            else
                return _applicationDbContext.Products.Where(p => p.CategoryId == id).Include(p => p.supplier).ToList();
        }

        public List<Product> GetFilteredByName(string name)//helper method
        {
            return _applicationDbContext.Products.Where(p => p.Name.Contains(name)).Include(p => p.supplier).ToList();
        }

        public List<Product> GetFilteredByNameWithCategory(string name, int? id)
        {
            if (id == null)
                return GetFilteredByName(name);
            else
                return _applicationDbContext.Products.Where(p => p.Name.Contains(name) && p.CategoryId == id).Include(p => p.supplier).ToList();
        }

        public List<Product> GetFilteredByStatus(string staus, int? id)
        {
            if (staus == "Safe")
                return _applicationDbContext.Products.Where(p => p.StockQuantity > GlobalVariables.threshold && (id.HasValue ? p.CategoryId == id.Value : true)).Include(p => p.supplier).ToList();
            else if (staus == "Low")
                return _applicationDbContext.Products.Where(p => p.StockQuantity <= GlobalVariables.threshold && (id.HasValue ? p.CategoryId == id.Value : true)).Include(p => p.supplier).ToList();
            else
                return GetFilteredByCategory(id);
        }

        public List<int>? GetLowQuantitesProducts()
        {
                return _applicationDbContext.Products.Where(p => p.StockQuantity <= GlobalVariables.threshold).Select(p=>p.ID).ToList();
        }

        public List<Product> GetByIds(List<int> ids)
        {
            List<Product> products = new List<Product>();
            foreach (var id in ids)
            {
                products.Add(_applicationDbContext.Products.FirstOrDefault(p => p.ID == id));
            }
            return products;
        }
        public Product GetById(int id)
        {
            return _applicationDbContext.Products.FirstOrDefault(p => p.ID == id)!;
        }

        public void Save()
        {
            _applicationDbContext.SaveChanges();
        }

        public void Update(Product product)
        {
            _applicationDbContext.Products.Update(product);
        }

        public int GetProductCount()
        {
            return _applicationDbContext.Products.Count();
        }

        public int GetItemsCount()
        {
            return _applicationDbContext.Products.Sum(e => e.StockQuantity);
        }

        public Product GetByName(string name)
        {
            return _applicationDbContext.Products.FirstOrDefault(p => p.Name == name);
        }
    }
}
