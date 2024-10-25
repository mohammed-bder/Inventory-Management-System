
namespace Inventory_Management_System.Repository.repo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public CategoryRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }


        public void Add(Category entity)
        {
            applicationDbContext.Add(entity);
        }

        public void Delete(Category entity)
        {
            applicationDbContext.Remove(entity);
        }

        public List<Category> GetAll()
        {
            return applicationDbContext.Categories.ToList();
        }
        public int GetAllCount()
        {
            return applicationDbContext.Categories.Count();
        }
        public Category GetById(int id)
        {
            return applicationDbContext.Categories.FirstOrDefault(e => e.ID == id);
        }

        public void Save()
        {
            applicationDbContext.SaveChanges();
        }

        public void Update(Category entity)
        {
            applicationDbContext.Update(entity);
        }

        public void DeleteCategories(List<int> categoryIds)
        {
            foreach (int categoryId in categoryIds)
            {
                Category category = GetById(categoryId);
                Delete(category);
            }
            Save();
        }

        public List<Category> GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return applicationDbContext.Categories.ToList(); // Return all categories if no search string
            }

            // Perform case-insensitive search by converting both the Name and the search string to lowercase
            var lowerCaseSearchString = name.ToLower();

            // Use Contains for partial matching and case-insensitivity
            return applicationDbContext.Categories
                           .Where(c => c.Name.ToLower().Contains(lowerCaseSearchString))
                           .ToList();
        }

    }
}
