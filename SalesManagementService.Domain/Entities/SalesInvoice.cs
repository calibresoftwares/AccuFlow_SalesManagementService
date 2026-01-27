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

        [Required]
        public Guid CustomerId { get; set; }

        public Guid? SalesOrderId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int Status { get; set; } // 0 = Draft, 1 = Pending, 2 = Paid, 3 = Cancelled, etc.

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        // Navigation
        public ICollection<SalesInvoiceLineItem> LineItems { get; set; } = new List<SalesInvoiceLineItem>();
    }
}
