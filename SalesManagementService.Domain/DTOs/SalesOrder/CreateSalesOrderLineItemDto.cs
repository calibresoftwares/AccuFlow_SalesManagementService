using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.DTOs.SalesOrder
{
    public class CreateSalesOrderLineItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal UnitPrice { get; set; }
        
        public decimal Discount { get; set; } = 0;
        
        public decimal Tax { get; set; } = 0;
        
        public decimal Total { get; set; }
        
        public Guid TenantId { get; set; }
    }
}
