using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Entities
{
    public class SalesOrderLineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid LineItemId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SalesOrderId { get; set; }

        [Required]
        public int ProductId { get; set; } // From Inventory Service

        [Required]
        public int Quantity { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } // (Quantity * UnitPrice) - Discount + Tax

        public Guid TenantId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public SalesOrder SalesOrder { get; set; }
    }
}
