using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SalesManagementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_sales_Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerName = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    Email = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    ContactPerson = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    MobileNumber = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    BillingAddress = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    ShippingAddress = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    GSTNumber = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    PANNumber = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    TaxCategory = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    TDSPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    CustomDutyPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    PaymentTerms = table.Column<string>(type: "text", nullable: false, collation: "case_insensitive"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
