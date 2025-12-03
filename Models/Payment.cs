using System.ComponentModel.DataAnnotations;

namespace StockAssist.Web.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public PaymentMethod Method { get; set; } = PaymentMethod.Card;
        public bool Succeeded { get; set; } = false;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime? PaidAt { get; set; }

    }
    public enum PaymentMethod
    {
        Card,
        BankTransfer,
        Cash
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }
}