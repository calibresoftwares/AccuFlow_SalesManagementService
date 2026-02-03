using System;
using System.ComponentModel.DataAnnotations;

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
        
        public decimal TaxAmount { get; set; } = 0;
        
        public decimal Total { get; set; }
        
       
        
        public Guid CreatedBy { get; set; }
    }
}

