using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.DTOs.SalesInvoice
{
    public class CreateSalesInvoiceLineItemDto
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        public decimal Quantity { get; set; }
        
        [Required]
        public decimal UnitPrice { get; set; }
        
        public decimal Discount { get; set; } = 0;
        
        [Required]
        public decimal TaxPercentage { get; set; } = 0;
        
        public decimal TaxAmount { get; set; } = 0;
        
        public decimal Total { get; set; }
        
        public Guid TenantId { get; set; }
    }
}
