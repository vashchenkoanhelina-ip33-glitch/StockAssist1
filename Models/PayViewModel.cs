using System;

namespace StockAssist.Web.Models
{

    public class PayViewModel
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        
        public decimal StoragePricePerMonth { get; set; }
        
        public int StorageMonths { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public string? PaymentMethod { get; set; }

        public string? CardNumber { get; set; }

        public string? CardExpiry { get; set; }

        public string? CardCvv { get; set; }
        
        public string? CardBrand { get; set; }


        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? BillingZip { get; set; }
        public string? BillingCountry { get; set; }
        public string? BillingCity { get; set; }
        public string? BillingAddress { get; set; }

        public string? Phone { get; set; }

        public bool SaveCard { get; set; }

        public string? SavedCardLast4 { get; set; }
        public string? SavedCardBrand { get; set; }
        public string? SavedCardExpiry { get; set; }
    }
}
