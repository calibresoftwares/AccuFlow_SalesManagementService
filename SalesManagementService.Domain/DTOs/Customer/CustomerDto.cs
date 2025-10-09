using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.DTOs.Customer
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }
        public string GSTNumber { get; set; }
        public string PANNumber { get; set; }
        public string TaxCategory { get; set; } // Regular, Composition, Exempt
        public decimal TDSPercentage { get; set; }
        public decimal CustomDutyPercentage { get; set; }
        public string PaymentTerms { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Guid TenantId { get; set; }
    }
}
