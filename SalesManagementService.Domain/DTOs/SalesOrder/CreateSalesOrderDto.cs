using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.DTOs.SalesOrder
{
    public class CreateSalesOrderDto
    {
        [Required]
        public int CustomerId { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        public string OrderNumber { get; set; }
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        public decimal Discount { get; set; } = 0;
        
        public decimal Tax { get; set; } = 0;
        
        public string Status { get; set; } = "Pending";
        
        public Guid TenantId { get; set; }
        
        public List<CreateSalesOrderLineItemDto> SalesOrderLineItems { get; set; } = new List<CreateSalesOrderLineItemDto>();
    }
}
