using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesManagementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_SalesInvoice_Add_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "SalesInvoiceLineItems",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "SalesInvoiceLineItems");
        }
    }
}
