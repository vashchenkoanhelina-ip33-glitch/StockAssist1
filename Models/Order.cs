using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockAssist.Web.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserAccountId { get; set; } = null!;
        public ApplicationUser? UserAccount { get; set; }

        public string? ServiceType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Awaiting;

        public int? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public double? WeightKg { get; set; }
        
        public double? VolumeM3 { get; set; }

        public decimal StoragePricePerMonth { get; set; }

        public int StorageMonths { get; set; }

        public DateTime? StorageFrom { get; set; }
        
        public DateTime? StorageTo { get; set; }

        public ICollection<OrderItem>? Items { get; set; }

        public Payment? Payment { get; set; }

        public bool IsPaid { get; set; } = false;

        public string? Notes { get; set; }
        public bool ReminderEmailSent { get; set; }
        
    }

    public enum OrderStatus
    {
        Awaiting,
        InProgress,
        Completed,
        Canceled,
        Done
    }
}