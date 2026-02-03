using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Entities
{
    public class SalesInvoiceLineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid SalesInvoiceLineItemId { get; set; } = Guid.NewGuid();

        public Guid SalesInvoiceId { get; set; } // Link to SalesInvoice Parent

        public Guid ProductId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } // Delivered / Final billed qty

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } // (Qty * Price - Discount + Tax)

        public Guid TenantId { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public SalesInvoice SalesInvoice { get; set; }
    }
}

