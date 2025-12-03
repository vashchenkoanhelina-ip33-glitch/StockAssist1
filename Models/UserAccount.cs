namespace StockAssist.Web.Models
{
    public class UserAccount
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Email { get; set; }
        public string? PasswordHash { get; set; }

        public string? Role { get; set; } = "User";

        public string? PaymentFrequency { get; set; } = "Monthly";
        public DateTime? LastPaymentDate { get; set; }
    }
}