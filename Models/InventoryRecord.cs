namespace StockAssist.Web.Models
{
    public class InventoryRecord
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
