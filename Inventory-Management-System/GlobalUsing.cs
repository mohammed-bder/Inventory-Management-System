global using Inventory_Management_System.Data;
global using Inventory_Management_System.Models;
global using Inventory_Management_System.Controllers;
global using Inventory_Management_System.ViewModel;
global using Microsoft.AspNetCore.Mvc.TagHelpers;

public static class GlobalVariables
{
    public static int threshold { get; set; } = 5;
    public static int AlertFactor { get; set; } = 10;

}

