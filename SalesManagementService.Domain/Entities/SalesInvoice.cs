using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Entities
{
    public class SalesInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid SalesInvoiceId { get; set; } = Guid.NewGuid();

        public string InvoiceNumber { get; set; }

        public Guid? SalesOrderId { get; set; } // Link to SO (optional but recommended)

        // Customer details
        public Guid CustomerId { get; set; }

        // Financial details
        public DateTime InvoiceDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Gross total (before tax/discount)

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; } // Final amount = Total - Discount + Tax

        public InvoiceStatus Status { get; set; }

        // Audit fields
        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Multi-tenant fields
        public Guid TenantId { get; set; }

        // Line Items
        public List<SalesInvoiceLineItem> LineItems { get; set; }
    }
}

