using Inventory_Management_System.Models;
using Inventory_Management_System.Repository;
using Inventory_Management_System.Repository.repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class EmployeeSupplierController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IEmployeeSupplierRepository employeeSupplierRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IAlertRepository alertRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public EmployeeSupplierController(IEmployeeSupplierRepository employeeSupplier,
                                        IProductRepository productRepository,
                                        ISupplierRepository supplierRepository,
                                        IEmployeeRepository employeeRepository,
                                        IAlertRepository alertRepository,
                                        UserManager<ApplicationUser> userManager)
        {
            this.employeeSupplierRepository = employeeSupplier;
            this._productRepository = productRepository;
            this.supplierRepository = supplierRepository;
            this.employeeRepository = employeeRepository;
            this.alertRepository = alertRepository;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            List<EmployeeSupplier> employeeSuppliers = employeeSupplierRepository.GetAll();
            List<string> productNames = new List<string>();
            Product? product = new Product();
            foreach (var item in employeeSuppliers)
            {
                product = _productRepository.GetById(item.ProductIdentifier);
                if (product != null)
                {
                    productNames.Add(product.Name);
                }
            }

            ViewBag.Products1 = productNames;
            return View("Index", employeeSuppliers);
        }
        
        public IActionResult NewlyCreatedReceipts()
        {
            List<EmployeeSupplier> suppliers = new List<EmployeeSupplier>();
            List<string> product_Names = new List<string>();   //save the product name of the receipt
            EmployeeSupplier? sup = new EmployeeSupplier();
            List<Product> products = _productRepository.GetAll();
            foreach (var prod in products)
            {
                if (prod.ModifiedDate.HasValue) // Check if ModifiedDate is not null
                {
                    sup = employeeSupplierRepository.GetByDate(prod.ModifiedDate.Value);
                    if (sup != null)
                    {
                        suppliers.Add(sup);
                        product_Names.Add(prod.Name);
                    }
                }
            }
            ViewBag.Products0 = product_Names;
            return View("NewlyCreatedReceipts", suppliers);
        }

        [HttpGet]
        public async  Task<IActionResult> Add()
        {
            var myUser = await userManager.GetUserAsync(User);
            ViewBag.EmpId = myUser.Employee_id;
            ViewBag.Productlist = new SelectList(_productRepository.GetAll(), "ID", "Name");
            ViewBag.Supplierlist = new SelectList(supplierRepository.GetAll(), "ID", "Name");
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(EmployeeSupplier employeeFromRequest)
        {
            if (ModelState.IsValid)
            {
                employeeFromRequest.StartDate = DateTime.Now;
                employeeSupplierRepository.Add(employeeFromRequest);
                employeeSupplierRepository.Save();
                Product? product = _productRepository.GetById(employeeFromRequest.ProductIdentifier);
                if(product !=null)
                {
                    //update product
                    product.StockQuantity += employeeFromRequest.Quantity;
                    product.UnitPrice = (employeeFromRequest.TotalCost/employeeFromRequest.Quantity)*1.05;
                    product.ModifiedDate = employeeFromRequest.StartDate;
                    product.SupplierId = employeeFromRequest.SupplierID;
                    _productRepository.Update(product);
                    _productRepository.Save();

                    if (product.StockQuantity > product.ReorderLevel)
                    {
                        StartAlert? startAlert = alertRepository.GetFalseByProductId(employeeFromRequest.ProductIdentifier);
                        if (startAlert != null)
                        {
                            startAlert.IsResolved = true;
                            startAlert.ResolveDate = employeeFromRequest.StartDate;
                            alertRepository.Update(startAlert);
                            alertRepository.Save();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Product not found");
                }
                return RedirectToAction("Index");
            }
            return View("Add");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != null)
            {
                var myUser = await userManager.GetUserAsync(User);
                ViewBag.EmpId = myUser.Employee_id;

                EmployeeSupplier employeeSupplier = employeeSupplierRepository.GetById(id);
                EmployeeSupplierWithSupplierList EmpSupViewModel = new EmployeeSupplierWithSupplierList();
                
                EmpSupViewModel.Id = employeeSupplier.Id;
                EmpSupViewModel.TotalCost = employeeSupplier.TotalCost;
                EmpSupViewModel.Quantity = employeeSupplier.Quantity;
                EmpSupViewModel.EmployeeID = employeeSupplier.EmployeeID;
                EmpSupViewModel.SupplierID = employeeSupplier.SupplierID;
                EmpSupViewModel.ProductIdentifier = employeeSupplier.ProductIdentifier;
                

                EmpSupViewModel.Employees = employeeRepository.GetAll();
                EmpSupViewModel.Suppliers = supplierRepository.GetAll();
                EmpSupViewModel.Products = _productRepository.GetAll();

                return View("Edit", EmpSupViewModel);
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public IActionResult SaveEdit(EmployeeSupplierWithSupplierList employeeFromRequest)
        {
            DateTime oldDate;
            if (ModelState.IsValid)
            {
                EmployeeSupplier? employeeSupplier = employeeSupplierRepository.GetById(employeeFromRequest.Id);
                if (employeeSupplier != null)
                {

                    //Save old date then update it
                    oldDate = employeeSupplier.StartDate;
                    employeeSupplier.StartDate = DateTime.Now;

                    //if there was a change in the product
                    Product? product = _productRepository.GetById(employeeFromRequest.ProductIdentifier);
                    if (product != null)
                    {
                        //if there was a change in the unit cost
                        if (employeeSupplier.TotalCost != employeeFromRequest.TotalCost)
                        {
                            product.UnitPrice = (employeeFromRequest.TotalCost / employeeFromRequest.Quantity) * 1.05;
                            employeeSupplier.TotalCost = employeeFromRequest.TotalCost;
                        }
                        //if there was a change in the Quantity
                        //            old                            new
                        if (employeeSupplier.Quantity != employeeFromRequest.Quantity)
                        {
                            //the user increases the quantity
                            if(employeeFromRequest.Quantity > employeeSupplier.Quantity)
                            {
                                product.StockQuantity += (employeeFromRequest.Quantity - employeeSupplier.Quantity);
                                employeeSupplier.Quantity = employeeFromRequest.Quantity;
                            }
                            //the user decreases the quantity
                            else
                            {
                                if (((employeeFromRequest.Quantity - employeeSupplier.Quantity) + product.StockQuantity) > 0)
                                {
                                    //the ordered quantity were large by mistake but can be retreived
                                    product.StockQuantity += (employeeFromRequest.Quantity - employeeSupplier.Quantity);
                                    employeeSupplier.Quantity = employeeFromRequest.Quantity;
                                }
                                else
                                {
                                    //the ordered quantity were large by mistake but they where already sold
                                    //you can only edit the quantity to what only was left
                                    employeeSupplier.Quantity = (employeeSupplier.Quantity - product.StockQuantity);
                                    product.StockQuantity -= employeeSupplier.Quantity;
                                }
                            }





                            //Check if quantity less than the threshold and alert
                            //           new quantity          threshold
                            if (product.StockQuantity > product.ReorderLevel)
                            {
                                StartAlert? startAlert = alertRepository.GetFalseByProductId(employeeFromRequest.ProductIdentifier);
                                if (startAlert != null)
                                {
                                    startAlert.IsResolved = true;
                                    startAlert.ResolveDate = employeeSupplier.StartDate;
                                    alertRepository.Update(startAlert);
                                    alertRepository.Save();
                                }
                            }
                            //             new quantity             threshold
                            else
                            {
                                StartAlert? startAlert = alertRepository.GetByProductIdAndDate(employeeFromRequest.ProductIdentifier, oldDate);
                                if (startAlert != null)
                                {
                                    startAlert.IsResolved = false;
                                    startAlert.ResolveDate = null;
                                    alertRepository.Update(startAlert);
                                    alertRepository.Save();
                                }
                            }

                            

                        }

                        product.SupplierId = employeeFromRequest.SupplierID;
                        //update product
                        product.ModifiedDate = employeeSupplier.StartDate;
                        _productRepository.Update(product);
                        _productRepository.Save();

                    }
                    else
                    {
                        ModelState.AddModelError("", "Product not found");
                    }

                    

                    

                    //info must be updated in the receipt

                    employeeSupplier.SupplierID = employeeFromRequest.SupplierID;
                    employeeSupplier.EmployeeID = employeeFromRequest.EmployeeID;
                    employeeSupplier.ProductIdentifier = employeeFromRequest.ProductIdentifier;

                    employeeSupplierRepository.Update(employeeSupplier);
                    employeeSupplierRepository.Save();


                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Product not found");
                }
            }
            return View("Edit", employeeFromRequest);
        }


        [HttpPost]
        public IActionResult Delete(List<int> employeeIds)
        {
            EmployeeSupplierWithIdListViewModel employeeWithId = new EmployeeSupplierWithIdListViewModel();
            employeeWithId.employeeSuppliers = new List<EmployeeSupplier>();
            employeeWithId.employeeSupplierIds = employeeIds;
            employeeWithId.productNames = new List<string>();
            Product product = new Product();

            foreach (int id in employeeIds)
            {
                EmployeeSupplier emp = new EmployeeSupplier();
                emp = employeeSupplierRepository.GetById(id);
                if (emp != null)
                {
                    employeeWithId.employeeSuppliers.Add(emp);
                    product = _productRepository.GetById(emp.ProductIdentifier);
                    if (product != null)
                    {
                        employeeWithId.productNames.Add(product.Name);
                    }
                }
            }
            return View("Delete", employeeWithId);
        }


        [HttpPost]
        public IActionResult deleteConfirmed(List<int> employeeIds)
        {
            // Check the received IDs and perform deletion logic
            if (employeeIds != null && employeeIds.Any())
            {
                // Example: Delete employees by their IDs from the database
                employeeSupplierRepository.DeleteEmployeeSuppliers(employeeIds);
                return View("deleteConfirmed");  // Redirect back to the employee list
            }
            return View("Error");  // Handle the case where no IDs are passed
        }
        
        [HttpPost]
        public IActionResult deleteLogicConfirmed(List<int> employeeIds)
        {
            EmployeeSupplier? supplier = new EmployeeSupplier();
            Product? product = new Product();
            StartAlert? startAlert = new StartAlert();
            // Check the received IDs and perform deletion logic
            if (employeeIds != null && employeeIds.Any())
            {
                foreach(var item in employeeIds)
                {
                    supplier = employeeSupplierRepository.GetById(item);
                    if(supplier != null)
                    {
                        product = _productRepository.GetById(supplier.ProductIdentifier);
                        if(product != null)
                        {
                            if(product.StockQuantity >= supplier.Quantity)
                            {
                                //stock quantity will be less than threshold after delete
                                product.StockQuantity -= supplier.Quantity;
                                startAlert = alertRepository.GetByProductIdAndDate(supplier.ProductIdentifier, supplier.StartDate);
                                if (startAlert != null)
                                {
                                    if(product.StockQuantity < product.ReorderLevel)
                                    {
                                        startAlert.IsResolved = false;
                                        startAlert.ResolveDate = null;
                                        alertRepository.Update(startAlert);
                                    }
                                }
                            }
                            else
                            {
                                //error must be held but on consecutive delete the quantity will equal 0
                                //we force the user to not delete from what was already sold
                                product.StockQuantity = 0;
                                startAlert = alertRepository.GetByProductIdAndDate(supplier.ProductIdentifier, supplier.StartDate);
                                if(startAlert != null)
                                {
                                    startAlert.IsResolved = false;
                                    startAlert.ResolveDate = null;
                                    alertRepository.Update(startAlert);
                                }
                            }
                            _productRepository.Update(product);
                        }
                        employeeSupplierRepository.Delete(supplier);
                    }
                }

                //Save changes
                alertRepository.Save();
                _productRepository.Save();
                employeeSupplierRepository.Save();

                // Example: Delete employees by their IDs from the database
                //employeeSupplierRepository.DeleteEmployeeSuppliers(employeeIds);
                return View("deleteConfirmed");  // Redirect back to the employee list
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

            Product product = _productRepository.GetByName(StringFromRequest);

            List<EmployeeSupplier> employeeSuppliers = new List<EmployeeSupplier>();
            employeeSuppliers = employeeSupplierRepository.GetAllByProductId(product.ID);

            List<string> Names = new List<string>();

            for (int i = 0; i < employeeSuppliers.Count; i++)
            {
                Names.Add(product.Name);
            }

            ViewBag.Products1 = Names;
            return View("Index", employeeSuppliers);
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            string FullName;
            // Fetch the data to export
            var employeeSuppliers = employeeSupplierRepository.GetAll();

            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Employee Suppliers");

                // Add headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Employee Name";
                worksheet.Cells[1, 3].Value = "Supplier Name";
                worksheet.Cells[1, 4].Value = "Product Name";
                worksheet.Cells[1, 5].Value = "Quantity";
                worksheet.Cells[1, 6].Value = "Total Cost";
                worksheet.Cells[1, 7].Value = "Start Date";


                // Add data to the worksheet
                for (int i = 0; i < employeeSuppliers.Count; i++)
                {
                    var employeeSupplier = employeeSuppliers[i];
                    // Get the product details
                    var product = _productRepository.GetById(employeeSupplier.ProductIdentifier);
                    string productName = product != null ? product.Name : "Product Not Found";

                    worksheet.Cells[i + 2, 1].Value = employeeSupplier.Id;
                    FullName = employeeSupplier.Employee.FName + "_" + employeeSupplier.Employee.LName;
                    worksheet.Cells[i + 2, 2].Value = FullName;
                    worksheet.Cells[i + 2, 3].Value = employeeSupplier.Supplier.Name;
                    worksheet.Cells[i + 2, 4].Value = productName;
                    worksheet.Cells[i + 2, 5].Value = employeeSupplier.Quantity;
                    worksheet.Cells[i + 2, 6].Value = employeeSupplier.TotalCost;
                    worksheet.Cells[i + 2, 7].Value = employeeSupplier.StartDate.ToString("yyyy-MM-dd");
                }

                // Format the header
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Auto-fit columns for all cells
                worksheet.Cells.AutoFitColumns();

                // Convert to a byte array and return as a file
                var fileContents = package.GetAsByteArray();
                var fileName = "EmployeeSuppliers.xlsx";
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        } 
    }
}
