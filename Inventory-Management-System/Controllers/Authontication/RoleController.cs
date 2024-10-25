using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.ViewModel
{
    [Authorize(Roles = "Admin,admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public IActionResult AddRole()
        {
            return View("Add");
        }

        
        [HttpPost]
        public async Task<IActionResult> SaveRole(RoleViewModel RoleViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = RoleViewModel.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"The '{RoleViewModel.RoleName}' role has been added successfully!🎉";
                    return View("Add"); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("Add", RoleViewModel);
        }

    }
}
