using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockAssist.Web.Models.Admin
{
    public class WarehouseOverviewVm
    {
        public int? SelectedWarehouseId { get; set; }

        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public int TotalCells { get; set; }
        public int OccupiedCells { get; set; }
        public double OccupancyPercent =>
            TotalCells == 0 ? 0 :
                (double)OccupiedCells / TotalCells * 100.0;

        public int PaidOrders { get; set; }
        public int UnpaidOrders { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }

        public List<UserWarehouseStatVm> UserStats { get; set; } = new();
    }

    public class UserWarehouseStatVm
    {
        public string UserId { get; set; } = string.Empty;
        public string UserLabel { get; set; } = string.Empty;
        public int OrdersCount { get; set; }
        public int TotalItems { get; set; }
    }
}