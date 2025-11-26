using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Entities
{
    public class SalesOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid SalesOrderId { get; set; } = Guid.NewGuid();

        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; } // From Customer Service

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipped, Completed, Cancelled

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }

        public Guid TenantId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<SalesOrderLineItem> LineItems { get; set; }
    }
}
