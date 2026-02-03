using System;

namespace SalesManagementService.Domain.DTOs.SalesOrder
{
    public class SalesOrderLineItemDto
    {
        public Guid LineItemId { get; set; }
        public Guid SalesOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}