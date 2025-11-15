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
        
        [Required]
        public DateTime ExpectedDeliveryDate { get; set; }
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        public string Status { get; set; } = "Pending";
        
        public string CreatedBy { get; set; }
        
        public Guid TenantId { get; set; }
        
        public List<CreateSalesOrderLineItemDto> SalesOrderLineItems { get; set; } = new List<CreateSalesOrderLineItemDto>();
    }
}
