using Microsoft.AspNetCore.Identity;

namespace StockAssist.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public string? Role { get; set; }  = "User";

        public string? CardBrand { get; set; }
        public string? CardLast4 { get; set; }
        public string? CardExpiry { get; set; }

        public DateTime? LastPaymentDate { get; set; }
        public string? PaymentFrequency { get; set; }
        public string? SavedCardLast4 { get; set; }
        public string? SavedCardBrand { get; set; }
        public string? SavedCardExpiry { get; set; }

    }
}