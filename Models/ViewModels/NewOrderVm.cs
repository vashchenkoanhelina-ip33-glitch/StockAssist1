using StockAssist.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace StockAssist.Web.Models.ViewModels
{
    public class NewOrderVm
    {
        [Required]

        [Range(1, int.MaxValue, ErrorMessage = "Кількість повинна бути більшою за 0")]
        public int Quantity { get; set; }

        public string? Description { get; set; }
    }
}
