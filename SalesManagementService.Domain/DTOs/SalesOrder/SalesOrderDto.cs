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
        public int SalesOrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public List<SalesOrderLineItemDto> SalesOrderLineItems { get; set; } = new List<SalesOrderLineItemDto>();
    }
}