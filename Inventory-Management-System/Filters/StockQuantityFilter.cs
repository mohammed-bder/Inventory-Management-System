using Inventory_Management_System.Repository;
using Inventory_Management_System.Repository.repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Inventory_Management_System.Filters
{
    public class StockQuantityFilter : IAsyncActionFilter
    {
        private readonly IProductRepository productRepository;
        private readonly AlertController alertController;
        private readonly IAlertRepository alertRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StockQuantityFilter(IProductRepository productRepository,AlertController alertController,IAlertRepository alertRepository , UserManager<ApplicationUser> userManager)
        {
            this.productRepository = productRepository;
            this.alertController = alertController;
            this.alertRepository = alertRepository;
            this._userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            List<int>? LowProductsIds = productRepository.GetLowQuantitesProducts();//1 4 5 6 7

            List<int>? alertedProducts = alertRepository.GetAllAlertedProductsIds();//1 4 

            List<int>? notAlertedProducts = LowProductsIds.Except(alertedProducts).ToList();

            var myUser = await _userManager.GetUserAsync(context.HttpContext.User);
            if (notAlertedProducts != null)
            {
                foreach (int id in notAlertedProducts)
                {
                    
                    AlertWithEmployeesProductViewModel alertWithEmployeesProductViewModel = new AlertWithEmployeesProductViewModel();
                    alertWithEmployeesProductViewModel.AlertDate = DateTime.Now;
                    alertWithEmployeesProductViewModel.AlertQuantityLevel = GlobalVariables.threshold * GlobalVariables.AlertFactor;
                    alertWithEmployeesProductViewModel.IsResolved = false;

                   
                    alertWithEmployeesProductViewModel.EmployeeId = myUser.Employee_id??0; 
                    alertWithEmployeesProductViewModel.ProductId = id;

                    alertController.SaveAdd(alertWithEmployeesProductViewModel);

                }
            }

            await next();
        }
    }
}
