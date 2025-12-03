using System.ComponentModel.DataAnnotations;
namespace StockAssist.Web.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = "";
        [Range(1, 1000)] public int Width { get; set; }
        [Range(1, 1000)] public int Depth { get; set; }
        [Range(1, 1000)] public int Height { get; set; }
        public List<WarehouseCell> Cells { get; set; } = new();
    }

    public class WarehouseCell
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public bool IsOccupied { get; set; }
    }
}

