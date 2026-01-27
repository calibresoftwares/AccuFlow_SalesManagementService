using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.DTOs.SalesInvoice
{
    public class CreateSalesInvoiceDto
    {
        [Required]
        public Guid CustomerId { get; set; }
        
        public Guid? SalesOrderId { get; set; }
        
        public string InvoiceNumber { get; set; }
        
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int Status { get; set; } // 0 = Draft, 1 = Pending, 2 = Paid, 3 = Cancelled, etc.
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        public decimal DiscountAmount { get; set; } = 0;
        
        public decimal TaxAmount { get; set; } = 0;
        
        [Required]
        public decimal NetAmount { get; set; }
        
        [Required]
        public Guid TenantId { get; set; }
        
        [Required]
        public Guid CreatedBy { get; set; }
        
        public List<CreateSalesInvoiceLineItemDto> LineItems { get; set; } = new List<CreateSalesInvoiceLineItemDto>();
    }
}
