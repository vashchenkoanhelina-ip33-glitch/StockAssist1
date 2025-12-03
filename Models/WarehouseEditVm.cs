using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockAssist.Web.Models.Admin
{
    public class WarehouseEditVm
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Назва складу")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int Width { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Depth { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Height { get; set; }

        public List<int> SelectedProductIds { get; set; } = new();

        public List<SelectListItem> AllProducts { get; set; } = new();
    }
}