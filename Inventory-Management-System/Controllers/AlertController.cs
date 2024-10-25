using Inventory_Management_System.Models;
using Inventory_Management_System.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Drawing;
using System.IO;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class AlertController : Controller
    {
        private readonly IAlertRepository _alertRepo;
        private readonly IEmployeeRepository _empRepo;
        private readonly IProductRepository _productRepo;

        public AlertController(IAlertRepository alertRepo , IEmployeeRepository empRepo , IProductRepository productRepo) 
        {
            this._alertRepo = alertRepo;
            this._empRepo = empRepo;
            this._productRepo = productRepo;
        }

        public IActionResult Index()
        {
            List<StartAlert> startAlerts =  _alertRepo.GetAlertWithAllData();

          
            ViewBag.status = "";

            return View("index" , startAlerts);
        }
    
        public IActionResult Add()
        {
            AlertWithEmployeesProductViewModel viewModel = new AlertWithEmployeesProductViewModel()
            {
                
                Employees = _empRepo.GetAll(),
                Products = _productRepo.GetAll(),
                AlertDate = DateTime.Now,
            };

            return View("Add" , viewModel);
        }

        [HttpPost]
        public IActionResult SaveAdd(AlertWithEmployeesProductViewModel newAlertFReq)
        { 

            if(ModelState.IsValid)
            {
                StartAlert startAlert = new StartAlert()
                {
                    AlertDate = newAlertFReq.AlertDate,
                    AlertQuantityLevel = newAlertFReq.AlertQuantityLevel,
                    IsResolved = newAlertFReq.IsResolved,
                    EmployeeId = newAlertFReq.EmployeeId,
                    ProductId = newAlertFReq.ProductId,
                };

               _alertRepo.Add(startAlert);
                _alertRepo.Save();
                return RedirectToAction("Index");
            }
            return View("Add", newAlertFReq);
            
        }


        public IActionResult Update(int id)
        {
            StartAlert startAlert = _alertRepo.GetById(id); 
            AlertWithEmployeesProductViewModel viewModel = new AlertWithEmployeesProductViewModel()
            {
                ID = startAlert.ID,
                AlertQuantityLevel = startAlert.AlertQuantityLevel,
                EmployeeId = startAlert.EmployeeId,
                IsResolved = startAlert.IsResolved,
                ProductId = startAlert.ProductId,
                ResolveDate = startAlert.ResolveDate.GetValueOrDefault(),
                Employees = _empRepo.GetAll(),
                Products = _productRepo.GetAll(),
                AlertDate = DateTime.Now,
            };

            return View("Update", viewModel);
        }

        [HttpPost]
        public IActionResult SaveUpdate(int id ,AlertWithEmployeesProductViewModel updatedAlertFReq)
        {

            StartAlert alertFdb = _alertRepo.GetById(id);
            if (ModelState.IsValid)
            {
               
                if (alertFdb.AlertQuantityLevel != updatedAlertFReq.AlertQuantityLevel )
                    alertFdb.AlertQuantityLevel = updatedAlertFReq.AlertQuantityLevel;

                if (alertFdb.IsResolved != updatedAlertFReq.IsResolved)
                    alertFdb.IsResolved = updatedAlertFReq.IsResolved;

                if (alertFdb.EmployeeId != updatedAlertFReq.EmployeeId)
                    alertFdb.EmployeeId = updatedAlertFReq.EmployeeId;

                if (alertFdb.ProductId != updatedAlertFReq.ProductId)
                    alertFdb.ProductId = updatedAlertFReq.ProductId;


                if (updatedAlertFReq.IsResolved)
                    alertFdb.ResolveDate = DateTime.Now;


                _alertRepo.Update(alertFdb);
                _alertRepo.Save();
                return RedirectToAction("Index");
            }
            return View("Add", updatedAlertFReq);

        }


        public IActionResult Search(string name , string status)
        {
            List<StartAlert> startAlerts = _alertRepo.GetSearchAndStatusResult( name , status);


            if (status == "Pending")
                ViewBag.status = "Pending";
            else if (status == "Completed")
                ViewBag.status = "Completed";
            else
                ViewBag.status = "";

            return View("index", startAlerts);
        }


        [HttpGet]
        public IActionResult GetPendingAlerts()
        {
            var alert = _alertRepo.GetPendingAlert();
            var test = Json(alert);
            return PartialView("_NotificationCenterPartial", alert);
            //return Json(alert);

        }

        public IActionResult ExportToExcel()
        {
            string FullName;
            var alerts = _alertRepo.GetAlertWithAllData(); // Fetch alerts

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Alerts");

                // Set the title with styling
                var titleCell = worksheet.Cells[1, 1, 1, 7]; // Merge across seven columns
                titleCell.Merge = true;
                titleCell.Value = "Alerts List";
                titleCell.Style.Font.Color.SetColor(Color.White); // Font color
                titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Set fill pattern
                titleCell.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue); // Background color
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
                worksheet.Cells[2, 1].Value = "Alert ID";
                worksheet.Cells[2, 2].Value = "Alert Date";
                worksheet.Cells[2, 3].Value = "Employee Name";
                worksheet.Cells[2, 4].Value = "Product Name";
                worksheet.Cells[2, 5].Value = "Stock Quantity";
                worksheet.Cells[2, 6].Value = "Alert Quantity Level";
                worksheet.Cells[2, 7].Value = "Is Resolved";

                // Fill alert data with alternating row colors and borders
                for (int i = 0; i < alerts.Count; i++)
                {
                    int rowIndex = i + 3; // Starting from the third row
                    worksheet.Cells[rowIndex, 1].Value = alerts[i].ID;

                    // Set the alert date and format it as a date in Excel
                    worksheet.Cells[rowIndex, 2].Value = alerts[i].AlertDate;
                    worksheet.Cells[rowIndex, 2].Style.Numberformat.Format = "mm-dd-yyyy"; // Date format

                    FullName = alerts[i].employee.FName + " " + alerts[i].employee.LName;
                    worksheet.Cells[rowIndex, 3].Value = FullName;
                    worksheet.Cells[rowIndex, 4].Value = alerts[i].product.Name;
                    worksheet.Cells[rowIndex, 5].Value = alerts[i].product.StockQuantity;
                    worksheet.Cells[rowIndex, 6].Value = alerts[i].AlertQuantityLevel;

                    // Set status and color the cell based on its value
                    string status = alerts[i].IsResolved ? "Complete" : "Pending";
                    worksheet.Cells[rowIndex, 7].Value = status;

                    // Apply color based on status
                    if (!alerts[i].IsResolved)
                    {
                        worksheet.Cells[rowIndex, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);// Blue for Complete
                    }
                    else
                    {
                        worksheet.Cells[rowIndex, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 7].Style.Fill.BackgroundColor.SetColor(Color.OrangeRed); // OrangeRed for Pending
                    }

                    // Set border for each cell in the row
                    for (int j = 1; j <= 7; j++)
                    {
                        worksheet.Cells[rowIndex, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    if (i % 2 == 0)
                    {
                        worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                    }
                }

                // AutoFit columns for better visibility
                worksheet.Cells.AutoFitColumns();

                // Set the width for the first column to prevent overflow
                worksheet.Column(1).Width = 15; // Set the width for Alert ID

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Alerts.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}
