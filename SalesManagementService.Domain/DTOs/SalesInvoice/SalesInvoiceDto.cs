using System;
using System.Collections.Generic;
using SalesManagementService.Domain.Entities;

namespace SalesManagementService.Domain.DTOs.SalesInvoice
{
    public class SalesInvoiceDto
    {
        public Guid SalesInvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public Guid? SalesOrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public List<SalesInvoiceLineItemDto> LineItems { get; set; } = new List<SalesInvoiceLineItemDto>();
    }
}

