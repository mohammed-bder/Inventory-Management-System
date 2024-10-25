using Inventory_Management_System.Filters;
using Inventory_Management_System.Models;
using Inventory_Management_System.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Inventory_Management_System.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        private readonly IProductRepository productRepository;
        private readonly IEmployeeSupplierRepository employeeSupplierRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IProductTransactionRepository productTransactionRepository;
        private readonly ICategoryRepository categoryRepository;

        public HomeController(IProductRepository productRepository ,
                              IEmployeeSupplierRepository employeeSupplierRepository ,
                              IEmployeeRepository employeeRepository ,
                              ISupplierRepository supplierRepository,
                              ITransactionRepository transactionRepository,
                              IProductTransactionRepository productTransactionRepository,
                              ICategoryRepository categoryRepository)
        {
            this.productRepository = productRepository;
            this.employeeSupplierRepository = employeeSupplierRepository;
            this.employeeRepository = employeeRepository;
            this.supplierRepository = supplierRepository;
            this.transactionRepository = transactionRepository;
            this.productTransactionRepository = productTransactionRepository;
            this.categoryRepository = categoryRepository;
        }

        [ServiceFilter(typeof(StockQuantityFilter))]
        public ActionResult Index()
        {
            var dashboardCards = new List<DashboardCard>
            {
                new DashboardCard { Title = "Total Items", Value = productRepository.GetItemsCount().ToString(), Icon = "fa-cubes", BackgroundColor = "#6a5acd" , ActionLink = "/Product/Index" },
                new DashboardCard { Title = "Total Categories", Value = categoryRepository.GetAllCount().ToString(), Icon = "fa-list-alt", BackgroundColor = "#00bcd4" , ActionLink = "/Category/Index"},
                new DashboardCard { Title = "Total Products", Value = productRepository.GetProductCount().ToString() , Icon = "fa-box-open", BackgroundColor = "#4caf50" , ActionLink = "/Product/Index"},
                new DashboardCard { Title = "Total Orders", Value = employeeSupplierRepository.GetOrdersCount().ToString(), Icon = "fa-handshake", BackgroundColor = "#ff9800", ActionLink = "/EmployeeSupplier/Index" },
                new DashboardCard { Title = "Paid Transactions", Value = transactionRepository.GetTrabsactionsCount().ToString() , Icon = "fa-exchange-alt", BackgroundColor = "#4caf50", ActionLink = "/Transaction/Index" },
                new DashboardCard { Title = "Total Sales", Value = transactionRepository.GetTotalSells().ToString("C"), Icon = "fa-dollar-sign", BackgroundColor = "#2196f3", ActionLink = "/Transaction/Index" },
                new DashboardCard { Title = "Total Supplier", Value = supplierRepository.GetSupplierCount().ToString(), Icon = "fa-store", BackgroundColor = "#f44336", ActionLink = "/Supplier/Index" },
                new DashboardCard { Title = "Total Members", Value = employeeRepository.GetEmpCount().ToString(), Icon = "fa-users", BackgroundColor = "#00bcd4" , ActionLink = "/Employee/Index"}
            };

            List<TopEmployee> topEmployees = transactionRepository.GetTopEmployees();
            ViewBag.TopEmployees = topEmployees;

            List<TopSellingProduct> topSellingProducts = productTransactionRepository.GetTopSelling();
            ViewBag.TopSellingProducts = topSellingProducts;

            List<SalesHistory> salesHistory = transactionRepository.GetHistorySalesDictionary();
            ViewBag.SalesHistory = salesHistory;

            return View(dashboardCards);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        public IActionResult OurTeam()
        {
            return View("OurTeam");
        }
    }
}
