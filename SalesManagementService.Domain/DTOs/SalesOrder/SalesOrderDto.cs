using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.DTOs.SalesOrder
{
    public class SalesOrderDto
    {
        public Guid SalesOrderId { get; set; }
        public string OrderNumber { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal NetAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public List<SalesOrderLineItemDto> SalesOrderLineItems { get; set; } = new List<SalesOrderLineItemDto>();
    }
}