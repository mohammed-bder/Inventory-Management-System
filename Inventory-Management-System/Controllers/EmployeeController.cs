using Inventory_Management_System.Repository;
using Inventory_Management_System.Repository.repo;
using Inventory_Management_System.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Drawing;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public EmployeeController(IEmployeeRepository employeeRepository, UserManager<ApplicationUser> userManager)
        {
            this.employeeRepository = employeeRepository;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var myUser = await userManager.GetUserAsync(User);
            ViewBag.EmpId = myUser.Employee_id;
            List<Employee> employees = employeeRepository.GetAll();
            return View("Index", employees);
        }

        [HttpGet]
        public IActionResult Add()
        {

            return View("Add");
        }

        //[HttpPost]
        //public IActionResult SaveAdd(Employee employeeFromRequest)
        //{
        //    if ((employeeFromRequest.FName != null) && (employeeFromRequest.LName != null) && (ModelState.IsValid))
        //    {
        //        employeeFromRequest.CreatedDate = DateTime.Now;
        //        employeeRepository.Add(employeeFromRequest);
        //        employeeRepository.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return View("Add");
        //}

        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    if (id != null)
        //    {
        //        Employee EmpModel = employeeRepository.GetById(id);
        //        return View("Edit", EmpModel);
        //    }
        //    else
        //    {
        //        return View("Error");
        //    }
        //}

        //[HttpPost]
        //public IActionResult SaveEdit(Employee employeeFromRequest, int id)
        //{
        //    if ((employeeFromRequest.FName != null) && (employeeFromRequest.LName != null) && (ModelState.IsValid))
        //    {
        //        Employee EmpFromDB = employeeRepository.GetById(id);
        //        EmpFromDB.Email = employeeFromRequest.Email;
        //        EmpFromDB.CreatedDate = DateTime.Now;
        //        EmpFromDB.FName = employeeFromRequest.FName;
        //        EmpFromDB.LName = employeeFromRequest.LName;
        //        EmpFromDB.Phone = employeeFromRequest.Phone;
        //        employeeRepository.Update(EmpFromDB);
        //        employeeRepository.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return View("Edit", employeeFromRequest);
        //}

        //[HttpPost]
        //public IActionResult DeleteSelected(List<int> employeeIds)
        //{
        //    EmployeeWithIdListViewModel employeeWithId = new EmployeeWithIdListViewModel();
        //    employeeWithId.Employees = new List<Employee>();
        //    employeeWithId.employeeIds = employeeIds;
        //    foreach (int id in employeeIds)
        //    {
        //        Employee emp = new Employee();
        //        emp = employeeRepository.GetById(id);
        //        if (emp != null)
        //        {
        //            employeeWithId.Employees.Add(emp);
        //        }
        //    }
        //    return View("Delete", employeeWithId);
        //}

        //[HttpPost]
        //public IActionResult ddeleteConfirmed(int id)
        //{
        //    Employee employee = new Employee();
        //    employee = employeeRepository.GetById(id);
        //    employeeRepository.Delete(employee);
        //    employeeRepository.Save();
        //    return View("deleteConfirmed", employee);
        //}

        [HttpPost]
        public IActionResult deleteConfirmed(List<int> employeeIds)
        {
            List<Employee> employeeList = new List<Employee>();
            foreach (int id in employeeIds)
            {
                Employee emp = new Employee();
                emp = employeeRepository.GetById(id);
                if (emp != null)
                {
                    employeeList.Add(emp);
                }
            }
            // Check the received IDs and perform deletion logic
            if (employeeIds != null && employeeIds.Any())
            {
                // Example: Delete employees by their IDs from the database
                employeeRepository.DeleteEmployees(employeeIds);
                return View("deleteConfirmed", employeeList);  // Redirect back to the employee list
            }
            return View("Error");  // Handle the case where no IDs are passed
        }

        [HttpGet]
        public IActionResult Search(string StringFromRequest)
        {
            if (StringFromRequest == null)
            {
                return RedirectToAction("Index");
            }
            return View("Index", employeeRepository.GetByName(StringFromRequest));
        }

        
        [HttpGet]
        public IActionResult ExportToExcel()
        {
            var employees = employeeRepository.GetAll(); // Fetch employees

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Set the title with styling
                var titleCell = worksheet.Cells[1, 1, 1, 7]; // Merge across five columns
                titleCell.Merge = true;
                titleCell.Value = "Employees List";
                titleCell.Style.Font.Color.SetColor(Color.White); // Font color
                titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Set fill pattern
                titleCell.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Background color (same as in supplier export)
                titleCell.Style.Font.Size = 16; // Increase font size
                titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Center text
                titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Center vertically
                titleCell.Style.Font.Bold = true; // Bold title

                // Set column headers
                var headerRow = worksheet.Cells[2, 1, 2, 7]; // Set the range for headers
                headerRow.Style.Font.Bold = true; // Make headers bold
                headerRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRow.Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Header background color (same as supplier export)
                headerRow.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Add border

                // Set headers text
                worksheet.Cells[2, 1].Value = "Employee ID";
                worksheet.Cells[2, 2].Value = "First Name";
                worksheet.Cells[2, 3].Value = "Last Name";
                worksheet.Cells[2, 4].Value = "Salary";
                worksheet.Cells[2, 5].Value = "Email";
                worksheet.Cells[2, 6].Value = "Phone";
                worksheet.Cells[2, 7].Value = "Role";

                // Fill employee data with alternating row colors and borders
                for (int i = 0; i < employees.Count; i++)
                {
                    int rowIndex = i + 3; // Starting from the third row
                    worksheet.Cells[rowIndex, 1].Value = employees[i].ID;
                    worksheet.Cells[rowIndex, 2].Value = employees[i].FName;
                    worksheet.Cells[rowIndex, 3].Value = employees[i].LName;
                    worksheet.Cells[rowIndex, 4].Value = employees[i].Salary;
                    worksheet.Cells[rowIndex, 5].Value = employees[i].Email;
                    worksheet.Cells[rowIndex, 6].Value = employees[i].Phone;
                    worksheet.Cells[rowIndex, 7].Value = employees[i].Role;

                    // Set border for each cell in the row
                    for (int j = 1; j <= 7; j++)
                    {
                        worksheet.Cells[rowIndex, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    // Alternate row colors (same as supplier export)
                    if (i % 2 == 0)
                    {
                        worksheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                    }
                }

                // AutoFit columns
                worksheet.Cells.AutoFitColumns();

                // Set the width for the first column to prevent overflow
                worksheet.Column(1).Width = 15; // Set the width for Employee ID

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Employees.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }





    }
}
