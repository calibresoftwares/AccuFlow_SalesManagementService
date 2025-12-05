using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SalesManagementService.Domain.Entities;
using SalesManagementService.Infrastructure;

namespace SalesManagementService.Infrastructure.DatabaseManager
{
    public class SalesManagementDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesManagementDbContext(DbContextOptions<SalesManagementDbContext> options, IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderLineItem> SalesOrderLineItems { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceLineItem> SalesInvoiceLineItems { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _httpContextAccessor.HttpContext?.Items["ConnectionString"]?.ToString();

                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseNpgsql(connectionString);
                }
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>().UseCollation("case_insensitive");
            //configurationBuilder.Properties<string>().UseCollation("en_US.utf8");
            //configurationBuilder.Properties<string>().UseCollation("en_US.utf8"); // or another existing collation
            //configurationBuilder.Properties<string>().UseCollation("C.UTF-8");
            //configurationBuilder.Properties<string>().UseCollation("case_insensitive");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //try
            //{

            //    modelBuilder
            //    .HasCollation("case_insensitive", locale: "und-u-ks-level2", provider: "icu", deterministic: false)
            //    .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            base.OnModelCreating(modelBuilder);

            //    // Optional: Configure entities here, for example:
            //    modelBuilder.Entity<Category>().HasKey(c => c.CategoryID);

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //    throw new Exception(ex.Message);
            //}
            // modelBuilder.Entity<Product>()
            // .Property(p => p.CreatedDate)
            // .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // modelBuilder.Entity<Product>()
            //.Property(p => p.UpdatedDate)
            //.HasDefaultValueSql("CURRENT_TIMESTAMP");

            // modelBuilder.Entity<InventoryTransaction>()
            //     .Property(p => p.TransactionDate)
            //     .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // modelBuilder.Entity<ProductStockHistory>()
            //     .Property(p => p.ChangeDate)
            //     .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // modelBuilder.Entity<Stock>()
            //     .Property(p => p.LastUpdatedDate)
            //     .HasDefaultValueSql("CURRENT_TIMESTAMP");

            //modelBuilder.Entity<Product>()
            //.HasOne(p => p.Category)
            //.WithMany(c => c.Products)
            //.HasForeignKey(p => p.CategoryID)
            //.OnDelete(DeleteBehavior.Restrict);
        }

        private void LogException(Exception ex)
        {
            // Log to console
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");

            // Optionally, log to a file
            File.AppendAllText("Logs/migration-errors.log", $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n");
        }

    }
}
