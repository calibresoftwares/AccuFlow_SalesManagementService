using System;

namespace SalesManagementService.Domain.DTOs.SalesInvoice
{
    public class SalesInvoiceLineItemDto
    {
        public Guid SalesInvoiceLineItemId { get; set; }
        public Guid SalesInvoiceId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public Guid TenantId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

