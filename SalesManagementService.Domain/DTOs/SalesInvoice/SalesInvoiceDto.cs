using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.DTOs.SalesInvoice
{
    public class SalesInvoiceDto
    {
        public int Id { get; set; }
        public Guid SalesInvoiceId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? SalesOrderId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public Guid TenantId { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<SalesInvoiceLineItemDto> LineItems { get; set; } = new List<SalesInvoiceLineItemDto>();
    }
}
