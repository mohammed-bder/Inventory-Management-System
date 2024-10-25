using Inventory_Management_System.Repository;
using Inventory_Management_System.Repository.repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {

            List<Category> categories = categoryRepository.GetAll();
            return View("Index", categories);
        }

        [HttpGet]
        public IActionResult Add()
        {

            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Category categoryFromRequest)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.Add(categoryFromRequest);
                categoryRepository.Save();
                return RedirectToAction("Index");
            }
            return View("Add");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id != null)
            {
                Category CatModel = categoryRepository.GetById(id);
                return View("Edit", CatModel);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult SaveEdit(Category categoryFromRequest, int id)
        {
            if ((categoryFromRequest.Name != null) && (ModelState.IsValid))
            {
                Category CatFromDB = categoryRepository.GetById(id);
                CatFromDB.Name = categoryFromRequest.Name;
                CatFromDB.Description = categoryFromRequest.Description;

                categoryRepository.Update(CatFromDB);
                categoryRepository.Save();
                return RedirectToAction("Index");
            }
            return View("Edit", categoryFromRequest);
        }

        [HttpPost]
        public IActionResult Delete(List<int> categoryIds)
        {
            categoryRepository.DeleteCategories(categoryIds);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Search(string StringFromRequest)
        {
            if (String.IsNullOrEmpty(StringFromRequest))
            {
                return RedirectToAction("Index");
            }

            ViewData["CurrentFilter"] = StringFromRequest;
            var filteredCategories = categoryRepository.GetByName(StringFromRequest);

            return View("Index", filteredCategories);
        }

    }
}
