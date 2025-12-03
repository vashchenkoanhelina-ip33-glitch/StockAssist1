using System.ComponentModel.DataAnnotations;

namespace StockAssist.Web.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
    }
}