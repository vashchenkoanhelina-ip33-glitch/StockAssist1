using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockAssist.Web.Models
{
    public class WarehouseProduct
    {
        public int Id { get; set; }

        [Required]
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
    }
}