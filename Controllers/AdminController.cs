using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAssist.Web.Data;
using StockAssist.Web.Models;
using StockAssist.Web.Models.Admin;

namespace StockAssist.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Warehouses()
        {
            var warehouses = await _db.Warehouses
                .OrderBy(w => w.Name)
                .ToListAsync();

            return View(warehouses);
        }

        [HttpGet]
        public async Task<IActionResult> EditWarehouse(int? id)
        {
            if (id == null)
            {
                var vm = new WarehouseEditVm();
                return View(vm);
            }

            var wh = await _db.Warehouses.FindAsync(id);
            if (wh == null)
                return NotFound();

            var vmExisting = new WarehouseEditVm
            {
                Id = wh.Id,
                Name = wh.Name,
                Width = wh.Width,
                Depth = wh.Depth,
                Height = wh.Height
            };

            return View(vmExisting);
        }

        [HttpPost]
        public async Task<IActionResult> EditWarehouse(WarehouseEditVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Warehouse entity;

            if (model.Id.HasValue && model.Id.Value != 0)
            {
                entity = await _db.Warehouses.FindAsync(model.Id.Value);
                if (entity == null)
                    return NotFound();
            }
            else
            {
                entity = new Warehouse();
                _db.Warehouses.Add(entity);
            }

            entity.Name = model.Name;
            entity.Width = model.Width;
            entity.Depth = model.Depth;
            entity.Height = model.Height;

            await _db.SaveChangesAsync();

            return RedirectToAction("Warehouses");
        }

        [HttpGet]
        public async Task<IActionResult> WarehouseOverview(int? warehouseId)
        {
            var warehouses = await _db.Warehouses
                .OrderBy(w => w.Name)
                .ToListAsync();

            var vm = new WarehouseOverviewVm
            {
                SelectedWarehouseId = warehouseId,
                Warehouses = warehouses
                    .Select(w => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = w.Id.ToString(),
                        Text = w.Name,
                        Selected = (warehouseId == w.Id)
                    })
                    .ToList()
            };

            if (!warehouseId.HasValue)
                return View(vm);

            int wid = warehouseId.Value;

            vm.Orders = await _db.Orders
                .Include(o => o.UserAccount)
                .Include(o => o.Payment)
                .Where(o => o.WarehouseId == wid)
                .ToListAsync();

            var warehouse = warehouses.First(w => w.Id == wid);

            vm.TotalCells = warehouse.Width * warehouse.Depth * warehouse.Height;

            vm.OccupiedCells = await _db.WarehouseCells
                .Where(c => c.WarehouseId == wid && c.IsOccupied)
                .CountAsync();

            vm.PaidOrders = vm.Orders.Count(o =>
                o.IsPaid ||
                (o.Payment != null && o.Payment.Status == PaymentStatus.Paid));

            vm.UnpaidOrders = vm.Orders.Count - vm.PaidOrders;

            vm.PaidAmount = vm.Orders
                .Where(o => o.IsPaid ||
                            (o.Payment != null && o.Payment.Status == PaymentStatus.Paid))
                .Sum(o => o.StoragePricePerMonth * o.StorageMonths);

            vm.UnpaidAmount = vm.Orders
                .Where(o => !(o.IsPaid ||
                              (o.Payment != null && o.Payment.Status == PaymentStatus.Paid)))
                .Sum(o => o.StoragePricePerMonth * o.StorageMonths);
            
            vm.UserStats = vm.Orders
                .GroupBy(o => o.UserAccountId)
                .Select(g => new UserWarehouseStatVm
                {
                    UserId = g.Key,
                    UserLabel = g.First().UserAccount?.Email
                                ?? g.First().UserAccount?.UserName
                                ?? g.Key,
                    OrdersCount = g.Count(),
                    TotalItems = g.Sum(o => o.Quantity)
                })
                .OrderByDescending(x => x.TotalItems)
                .ToList();

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await _db.Warehouses
                .Include(w => w.Cells)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouse == null)
                return NotFound();

            var hasOrders = await _db.Orders.AnyAsync(o => o.WarehouseId == id);
            if (hasOrders)
            {
                TempData["Error"] =
                    $"Неможливо видалити склад \"{warehouse.Name}\", оскільки є замовлення, які його використовують.";
                return RedirectToAction("Warehouses");
            }

            var warehouseProducts = _db.WarehouseProducts
                .Where(x => x.WarehouseId == id);
            _db.WarehouseProducts.RemoveRange(warehouseProducts);

            if (warehouse.Cells != null && warehouse.Cells.Any())
            {
                _db.WarehouseCells.RemoveRange(warehouse.Cells);
            }

            _db.Warehouses.Remove(warehouse);

            await _db.SaveChangesAsync();

            TempData["Message"] = $"Склад \"{warehouse.Name}\" успішно видалено.";
            return RedirectToAction("Warehouses");
        }

    }
}
