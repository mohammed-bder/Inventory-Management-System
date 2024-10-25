using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) :base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeSupplier> EmployeeSuppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StartAlert> StartAlerts { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ProductTransaction> ProductTransactions { get; set; } 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Make sure Identity configurations are applied
            // Configuring composite key for EmployeeSupplier
            modelBuilder.Entity<EmployeeSupplier>()
                .HasKey(es => es.Id); // Id is now the primary key

            modelBuilder.Entity<EmployeeSupplier>()
                .HasOne(es => es.Employee)
                .WithMany(e => e.employeeSuppliers) // Assuming Employee has a collection of EmployeeSuppliers
                .HasForeignKey(es => es.EmployeeID);

            modelBuilder.Entity<EmployeeSupplier>()
                .HasOne(es => es.Supplier)
                .WithMany(s => s.employeeSuppliers) // Assuming Supplier has a collection of EmployeeSuppliers
                .HasForeignKey(es => es.SupplierID);

            modelBuilder.Entity<ProductTransaction>()
                .HasKey(pt => new { pt.ProductId, pt.TransactionId });

            modelBuilder.Entity<ProductTransaction>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTransactions)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductTransaction>()
                .HasOne(pt => pt.Transaction)
                .WithMany(t => t.ProductTransactions)
                .HasForeignKey(pt => pt.TransactionId);

            //// Ensure that ID is the primary key, and ProductId is a foreign key without unique constraint
            //modelBuilder.Entity<StartAlert>()
            //    .HasKey(sa => sa.ID); // Ensure ID is the primary key

            //modelBuilder.Entity<StartAlert>()
            //    .HasOne(sa => sa.product)
            //    .WithMany(p => p.startAlert) // Assuming Product has a collection of StartAlerts
            //    .HasForeignKey(sa => sa.ProductId)
            //    .OnDelete(DeleteBehavior.Restrict); // Optional: Prevent cascade delete
        }
    }
}
