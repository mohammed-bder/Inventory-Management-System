using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class AlertWithEmployeesProductViewModel
    {


        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime AlertDate { get; set; }
        public int AlertQuantityLevel { get; set; }
        public bool IsResolved { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ResolveDate { get; set; }


        public int EmployeeId { get; set; }


        public int ProductId { get; set; }


        public List<Employee>? Employees { get; set; }
        public List<Product>? Products { get; set; }
    }
}
