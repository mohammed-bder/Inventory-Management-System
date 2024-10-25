using Inventory_Management_System.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Drawing;
using System.IO;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ISupplierRepository supplierRepository;

        public SupplierController(ApplicationDbContext _Context, ISupplierRepository supplierRepository)
        {
            applicationDbContext = _Context;
            this.supplierRepository = supplierRepository;
        }

        public IActionResult Index()
        {
            var suppliers = supplierRepository.GetAll();
            return View("Index", suppliers);
        }

        public IActionResult Add()
        {
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                supplierRepository.Add(supplier);
                supplierRepository.Save();
                return RedirectToAction("Index");
            }
            return View("Add", supplier);
        }

        public IActionResult Edit(int id)
        {
            var supplier = supplierRepository.GetById(id);
            if (supplier != null)
            {
                return View("Edit" , supplier);
            }
            return Content("This Id Not Found");
        }

        [HttpPost]
        public IActionResult SaveEdit(Supplier supplier , int id)
        {
            if (ModelState.IsValid)
            {
                var existingSupplier = supplierRepository.GetById(id);
                if (existingSupplier != null)
                {
                    // Update the properties of the existing supplier with the new values
                    existingSupplier.Name = supplier.Name;
                    existingSupplier.Email = supplier.Email;
                    existingSupplier.Address = supplier.Address;
                    existingSupplier.Phone = supplier.Phone;
                    supplierRepository.Update(existingSupplier);
                    supplierRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            return View("Edit", supplier);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var supplier = supplierRepository.GetById(id);
            if (supplier != null)
            {
                supplierRepository.Delete(supplier);
                supplierRepository.Save();
                return RedirectToAction("Index");
            }
            return Content("This Id Not found");
        }

        public IActionResult SearchByName(string name)
        {
            ViewBag.SearchItem = name;
            if (string.IsNullOrWhiteSpace(name))
            {
                return NotFound("Please enter a valid name to search.");
            }

            var Supplier = supplierRepository.SearchByName(name);
            if (Supplier != null)
            {
                return View("Index", Supplier);
            }
            else
            {
                return NotFound("Instructor with the given name not found.");
            }
        }
        public IActionResult ExportToExcel()
        {
            var suppliers = supplierRepository.GetAll(); // Fetch suppliers

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Suppliers");

                // Set the title with styling
                var titleCell = worksheet.Cells[1, 1, 1, 5]; // Merge across five columns
                titleCell.Merge = true;
                titleCell.Value = "Suppliers List";
                titleCell.Style.Font.Color.SetColor(Color.White); // Font color
                titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Set fill pattern
                titleCell.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Background color
                titleCell.Style.Font.Size = 16; // Increase font size
                titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center text
                titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Center vertically
                titleCell.Style.Font.Bold = true; // Bold title

                // Set column headers
                var headerRow = worksheet.Cells[2, 1, 2, 5]; // Set the range for headers
                headerRow.Style.Font.Bold = true; // Make headers bold
                headerRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRow.Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Header background color
                headerRow.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Add border

                // Set headers text
                worksheet.Cells[2, 1].Value = "Supplier ID";
                worksheet.Cells[2, 2].Value = "Name";
                worksheet.Cells[2, 3].Value = "Email";
                worksheet.Cells[2, 4].Value = "Address";
                worksheet.Cells[2, 5].Value = "Phone";

                // Fill supplier data with alternating row colors and borders
                for (int i = 0; i < suppliers.Count; i++)
                {
                    int rowIndex = i + 3; // Starting from the third row
                    worksheet.Cells[rowIndex, 1].Value = suppliers[i].ID;
                    worksheet.Cells[rowIndex, 2].Value = suppliers[i].Name;
                    worksheet.Cells[rowIndex, 3].Value = suppliers[i].Email;
                    worksheet.Cells[rowIndex, 4].Value = suppliers[i].Address;
                    worksheet.Cells[rowIndex, 5].Value = suppliers[i].Phone;

                    // Set border for each cell in the row
                    for (int j = 1; j <= 5; j++)
                    {
                        worksheet.Cells[rowIndex, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    // Alternate row colors
                    if (i % 2 == 0)
                    {
                        worksheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                    }
                }

                // AutoFit columns
                worksheet.Cells.AutoFitColumns();

                // Set the width for the first column to prevent overflow
                worksheet.Column(1).Width = 15; // Set the width for Supplier ID

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Suppliers.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }



    }
}

