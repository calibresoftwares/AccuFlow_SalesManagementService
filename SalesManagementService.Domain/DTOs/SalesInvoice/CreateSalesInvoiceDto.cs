using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SalesManagementService.Domain.Entities;

namespace SalesManagementService.Domain.DTOs.SalesInvoice
{
    public class CreateSalesInvoiceDto
    {
        public string InvoiceNumber { get; set; }
        
        public Guid? SalesOrderId { get; set; }
        
        [Required]
        public Guid CustomerId { get; set; }
        
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        public decimal DiscountAmount { get; set; } = 0;
        
        public decimal TaxAmount { get; set; } = 0;
        
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
        
        [Required]
        public Guid CreatedBy { get; set; }
        
        [Required]
          
        public List<CreateSalesInvoiceLineItemDto> LineItems { get; set; } = new List<CreateSalesInvoiceLineItemDto>();
    }
}

