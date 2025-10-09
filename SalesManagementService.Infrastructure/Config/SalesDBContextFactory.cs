using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using SalesManagementService.Infrastructure;
using SalesManagementService.Infrastructure.DatabaseManager;
namespace SalesManagementService.Infrastructure.Config
{
    public class SalesDBContextFactory : IDesignTimeDbContextFactory<SalesManagementDbContext>
    {
        public SalesManagementDbContext CreateDbContext(string[] args)
        {
            // Use a default connection string for design-time purposes
            var defaultConnectionString = "Server=198.38.83.24;Database=Calibre_SalesManagementDB;Port=5432;Username=postgres; Password=C@l!bre#5700";

            var optionsBuilder = new DbContextOptionsBuilder<SalesManagementDbContext>();
            optionsBuilder.UseNpgsql(defaultConnectionString);

            // Pass null for IHttpContextAccessor as it's unavailable at design-time
            return new SalesManagementDbContext(optionsBuilder.Options, null);
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
