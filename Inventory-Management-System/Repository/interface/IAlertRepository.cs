using Inventory_Management_System.Data;

namespace Inventory_Management_System.Repository
{
    public interface IAlertRepository:IRepository<StartAlert>
    {
        public List<StartAlert> GetAlertWithAllData();
        public List<StartAlert> GetFilterByName(string? name);

        public List<StartAlert> GetStatus(string status);  

        public List<StartAlert> GetSearchAndStatusResult(string name, string status);
        public StartAlert GetFalseByProductId(int id);

        public List<StartAlert> GetPendingAlert();
        public List<int>? GetAllAlertedProductsIds();

        public StartAlert GetByProductIdAndDate(int id, DateTime date);

    }
}
