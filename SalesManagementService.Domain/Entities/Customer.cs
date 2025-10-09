using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Entities
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [Column(TypeName = "decimal(5,2)")]
        public decimal TDSPercentage { get; set; } // TDS deduction %

        [Column(TypeName = "decimal(5,2)")]
        public decimal CustomDutyPercentage { get; set; } // Import duty %
        public string PaymentTerms { get; set; } // Net 30, Net 60, etc.
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; }
        public Guid TenantId { get; set; }

        // Navigation properties

    }
}
