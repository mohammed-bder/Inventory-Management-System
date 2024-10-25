
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Inventory_Management_System.Repository.repo
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public TransactionRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }


        public void Add(Transaction entity)
        {
            applicationDbContext.Transactions.Add(entity);
        }

        public void Delete(Transaction entity)
        {
            applicationDbContext.Transactions.Remove(entity);
        }

        public List<Transaction> GetAll()
        {
            return applicationDbContext.Transactions.ToList();
        }
        public List<SalesHistory> GetHistorySalesDictionary()
        {
            return applicationDbContext.Transactions
                .GroupBy(t => new
                {
                    Date = t.Date.Date,   // Group by the date part
                    Hour = t.Date.Hour,   // Group by the hour part
                    Minute = t.Date.Minute // Group by the minute part
                })
                .Select(g => new
                {
                    Date = g.Key.Date,
                    Hour = g.Key.Hour,
                    Minute = g.Key.Minute,
                    Sales = g.Sum(t => t.TotalPrice) // Sum of sales for each minute
                })
                .AsEnumerable() // Switch to in-memory operations for further processing
                .Select(e => new SalesHistory
                {
                    // Format to include seconds (":00" at the end) because you want "HH:mm:ss"
                    date = $"{e.Date:yyyy-MM-dd} {e.Hour:D2}:{e.Minute:D2}:00",
                    Sales = (int)e.Sales
                })
                .OrderBy(s => s.date) // This uses the formatted string directly
                .ToList();
        }



        public int GetTrabsactionsCount()
        {
            return applicationDbContext.Transactions.Count();
        }
        public double GetTotalSells()
        {
            return applicationDbContext.Transactions.Sum(t=>t.TotalPrice);
        }
        public Transaction GetById(int id)
        {
            return applicationDbContext.Transactions.FirstOrDefault(t => t.ID == id);
        }

        public int GetLastTransactionId()
        {
            return applicationDbContext.Transactions.OrderByDescending(t => t.ID).FirstOrDefault().ID;
        }
        public List<TopEmployee> GetTopEmployees()
        {
            return applicationDbContext.Transactions.Include(t => t.employee).GroupBy(t => t.EmployeeId)
                .Select(e => new TopEmployee
                {
                    EmpId = e.Key,
                    EmpName = (e.FirstOrDefault().employee.FName + e.FirstOrDefault().employee.LName),
                    TotalSells = (double)e.Sum(t => (decimal)t.TotalPrice)
                })
                .OrderByDescending(e=>e.TotalSells).Take(5).ToList();
        }
        public void Save()
        {
            applicationDbContext.SaveChanges();
        }

        public void Update(Transaction entity)
        {
            applicationDbContext.Transactions.Update(entity);
        }
    }
}
