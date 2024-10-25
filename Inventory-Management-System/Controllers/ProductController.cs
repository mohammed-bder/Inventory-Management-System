using Inventory_Management_System.Filters;
using Inventory_Management_System.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Drawing;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ISupplierRepository supplierRepository;

        public ProductController(IProductRepository productRepository,ICategoryRepository categoryRepository,ISupplierRepository supplierRepository) 
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.supplierRepository = supplierRepository;
        }

        [ServiceFilter(typeof(StockQuantityFilter))]
        public IActionResult Index(int? id)
        {
            List<Product> products ;
            products = productRepository.GetFilteredByCategory(id).ToList();

            ViewBag.categories = categoryRepository.GetAll().ToList();
            ViewBag.selectedCategoryId = id;
            return View(products);
        }

        public IActionResult Search(string? name,int? id)
        {
            var products = productRepository.GetFilteredByNameWithCategory(name,id).ToList();

            ViewBag.categories = categoryRepository.GetAll().ToList();
            ViewBag.selectedCategoryId = id;
            ViewBag.name = name;
            return View("Index",products);
        }
        public IActionResult StatusFilter(string status, int? id)
        {
            var products = productRepository.GetFilteredByStatus(status, id).ToList();

            ViewBag.categories = categoryRepository.GetAll().ToList();
            ViewBag.selectedCategoryId = id;
            return View("Index", products);
        }

        public IActionResult Add()//in case of the product will added in first time in my stock
        {
            ProductWithCategoriesViewModel productWithCategories = new ProductWithCategoriesViewModel();
            productWithCategories.categories = categoryRepository.GetAll();
            productWithCategories.suppliers = supplierRepository.GetAll();
            return View(productWithCategories);
        }

        public IActionResult SaveAdd(ProductWithCategoriesViewModel productWithCategories)  
        {
            if (ModelState.IsValid)
            {
                Product product = new Product();

                product.Name = productWithCategories.Name;
                product.UnitPrice = productWithCategories.UnitPrice;
                product.CreatedDate = DateTime.Now;
                product.StockQuantity = 0;
                product.ReorderLevel = GlobalVariables.threshold;
                product.Description = productWithCategories.Description;
                product.CategoryId = productWithCategories.CategoryId;
                product.SupplierId = productWithCategories.SupplierId;

                if (!productRepository.CheckExistence(product.Name))
                {
                    productRepository.Add(product);
                    productRepository.Save();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Product Name Already found");
            }
            productWithCategories.categories = categoryRepository.GetAll();
            productWithCategories.suppliers = supplierRepository.GetAll();
            return View("Add",productWithCategories);
        }
        [HttpPost]
        public IActionResult Delete(string selectedIds)
        {
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var ids = selectedIds.Split(',').Select(int.Parse).ToList();

                foreach (int id in ids)
                {
                    Product product = productRepository.GetById(id);
                    productRepository.Delete(product);
                }
                productRepository.Save();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Product product = productRepository.GetById(id);
            ProductWithCategoriesViewModel productWithCategories = new ProductWithCategoriesViewModel();

            productWithCategories.ID = product.ID;
            productWithCategories.Name = product.Name;
            productWithCategories.UnitPrice = product.UnitPrice;
            productWithCategories.CreatedDate = product.CreatedDate;
            productWithCategories.Quantity = product.StockQuantity;
            productWithCategories.Description = product.Description;
            productWithCategories.CategoryId = product.CategoryId;
            productWithCategories.SupplierId = product.SupplierId;

            productWithCategories.categories = categoryRepository.GetAll();
            productWithCategories.suppliers = supplierRepository.GetAll();
            return View(productWithCategories);
        }

        [ServiceFilter(typeof(StockQuantityFilter))]
        public IActionResult SaveEdit(ProductWithCategoriesViewModel ProductWithCategoriesViewModel) 
        {
            if (ModelState.IsValid)
            {
                Product? product = productRepository.GetById(ProductWithCategoriesViewModel.ID??0);
                if(product != null)
                {
                    product.Name = ProductWithCategoriesViewModel.Name;
                    product.UnitPrice = ProductWithCategoriesViewModel.UnitPrice;
                    product.StockQuantity = ProductWithCategoriesViewModel.Quantity;
                    product.Description = ProductWithCategoriesViewModel.Description;
                    product.CategoryId = ProductWithCategoriesViewModel.CategoryId;
                    product.SupplierId = ProductWithCategoriesViewModel.SupplierId;
                    product.CreatedDate = ProductWithCategoriesViewModel.CreatedDate??DateTime.Now;
                    product.ModifiedDate = DateTime.Now;
                    product.ReorderLevel = GlobalVariables.threshold;

                    if (productRepository.CheckExistenceNameForEdit(product.Name))
                    {
                        productRepository.Update(product);
                        productRepository.Save();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Product Name Already found");

                        ProductWithCategoriesViewModel.categories = categoryRepository.GetAll();
                        ProductWithCategoriesViewModel.suppliers = supplierRepository.GetAll();
                        return View("Edit", ProductWithCategoriesViewModel);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Product not found");
                }
            }

            ProductWithCategoriesViewModel.categories = categoryRepository.GetAll();
            ProductWithCategoriesViewModel.suppliers = supplierRepository.GetAll();
            return View("Edit",ProductWithCategoriesViewModel);
        }

        public IActionResult ExportToExcel()
        {
            var products = productRepository.GetAll(); // Fetch all products

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Set the title with styling
                var titleCell = worksheet.Cells[1, 1, 1, 7]; // Merge across seven columns
                titleCell.Merge = true;
                titleCell.Value = "Products List";
                titleCell.Style.Font.Color.SetColor(Color.White); // Font color
                titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Set fill pattern
                titleCell.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Background color
                titleCell.Style.Font.Size = 16; // Increase font size
                titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center text
                titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Center vertically
                titleCell.Style.Font.Bold = true; // Bold title

                // Set column headers
                var headerRow = worksheet.Cells[2, 1, 2, 7]; // Set the range for headers
                headerRow.Style.Font.Bold = true; // Make headers bold
                headerRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRow.Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Header background color
                headerRow.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Add border

                // Set headers text
                worksheet.Cells[2, 1].Value = "Product ID";
                worksheet.Cells[2, 2].Value = "Name";
                worksheet.Cells[2, 3].Value = "Unit Price";
                worksheet.Cells[2, 4].Value = "Stock Quantity";
                worksheet.Cells[2, 5].Value = "Category";
                worksheet.Cells[2, 6].Value = "Supplier";
                worksheet.Cells[2, 7].Value = "Level"; // Add the new column for level

                // Fill product data with alternating row colors and borders
                for (int i = 0; i < products.Count; i++)
                {
                    int rowIndex = i + 3; // Starting from the third row
                    worksheet.Cells[rowIndex, 1].Value = products[i].ID;
                    worksheet.Cells[rowIndex, 2].Value = products[i].Name;
                    worksheet.Cells[rowIndex, 3].Value = products[i].UnitPrice;
                    worksheet.Cells[rowIndex, 4].Value = products[i].StockQuantity;
                    worksheet.Cells[rowIndex, 5].Value = products[i].category.Name;
                    worksheet.Cells[rowIndex, 6].Value = products[i].supplier.Name;

                    // Determine the level based on the StockQuantity and ReorderLevel
                    string level = products[i].StockQuantity >= products[i].ReorderLevel ? "Safe" : "Low";
                    worksheet.Cells[rowIndex, 7].Value = level;

                    // Set border for each cell in the row
                    for (int j = 1; j <= 7; j++)
                    {
                        worksheet.Cells[rowIndex, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    // Alternate row colors
                    if (i % 2 == 0)
                    {
                        worksheet.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                    }

                    // Apply conditional formatting based on the "Level" column (column 7)
                    if (level == "Safe")
                    {
                        worksheet.Cells[rowIndex, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGreen); // Green for safe
                    }
                    else
                    {
                        worksheet.Cells[rowIndex, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 7].Style.Fill.BackgroundColor.SetColor(Color.LightSalmon); // Red for low
                    }
                }

                // AutoFit columns
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Products.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}
