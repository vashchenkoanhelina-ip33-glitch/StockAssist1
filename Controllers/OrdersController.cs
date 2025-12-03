using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAssist.Web.Data;
using StockAssist.Web.Models;
using StockAssist.Web.Services;
using System.Security.Claims;

namespace StockAssist.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly WarehouseAllocator3D _allocator;
        private readonly IEmailService _emailService;

        public OrdersController(
            ApplicationDbContext db,
            WarehouseAllocator3D allocator,
            IEmailService emailService)
        {
            _db = db;
            _allocator = allocator;
            _emailService = emailService;
        }

        private decimal CalculateStoragePricePerItem(double? weightKg, double? volumeM3)
        {
            if (weightKg == null || volumeM3 == null || weightKg <= 0 || volumeM3 <= 0)
                return 0m;

            const double weightStep = 20.0;
            const double volumeStep = 4.0;
            const decimal basePrice = 1000m;

            var weightBlocks = Math.Ceiling(weightKg.Value / weightStep);
            var volumeBlocks = Math.Ceiling(volumeM3.Value / volumeStep);

            var bundles = Math.Max(weightBlocks, volumeBlocks);
            return (decimal)bundles * basePrice;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _db.Orders
                .Include(o => o.Payment)
                .Include(o => o.Warehouse)
                .Where(o => o.UserAccountId == userId &&
                            (o.Status == OrderStatus.Awaiting ||
                             o.Status == OrderStatus.InProgress))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> History(int? warehouseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var query = _db.Orders
                .Include(o => o.Payment)
                .Include(o => o.Warehouse)
                .Where(o => o.UserAccountId == userId);

            if (warehouseId.HasValue)
            {
                query = query.Where(o => o.WarehouseId == warehouseId.Value);
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            ViewBag.Warehouses = await _db.Warehouses
                .OrderBy(w => w.Name)
                .ToListAsync();

            ViewBag.SelectedWarehouseId = warehouseId;

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Warehouses = await _db.Warehouses.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string description,
            int quantity,
            string serviceType,
            DateTime? plannedDate,
            DateTime? endDate,
            double? weight,
            double? volume,
            int warehouseId,
            string? notes,
            PaymentMethod method = PaymentMethod.Card)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            if (quantity <= 0)
            {
                ModelState.AddModelError(nameof(quantity), "Кількість має бути більшою за 0.");
                ViewBag.Warehouses = await _db.Warehouses.ToListAsync();
                return View();
            }

            var warehouse = await _db.Warehouses.FirstOrDefaultAsync(w => w.Id == warehouseId);
            if (warehouse == null)
            {
                ModelState.AddModelError(string.Empty, "Обраний склад не знайдено.");
                ViewBag.Warehouses = await _db.Warehouses.ToListAsync();
                return View();
            }

            if (plannedDate.HasValue && endDate.HasValue && endDate < plannedDate)
            {
                ModelState.AddModelError(string.Empty,
                    "Кінцева дата зберігання не може бути раніше за дату початку.");
                ViewBag.Warehouses = await _db.Warehouses.ToListAsync();
                return View();
            }

            if (weight == null || weight <= 0 || volume == null || volume <= 0)
            {
                ModelState.AddModelError(string.Empty, "Вага та обʼєм мають бути більшими за 0.");
                ViewBag.Warehouses = await _db.Warehouses.ToListAsync();
                return View();
            }

            var volumePerBox = volume.Value;

            var side = Math.Ceiling(Math.Pow(volumePerBox, 1.0 / 3.0));
            var boxSize = (int)Math.Max(1, side);

            var wCells = boxSize * quantity;
            var dCells = boxSize;
            var hCells = boxSize;

            var placement = _allocator.FindInWarehouse(warehouseId, wCells, dCells, hCells);
            if (placement == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Склад переповнений. Оберіть інший або скористайтеся сервісом пізніше, вибачте за тимчасові незручності."
                );
                ViewBag.Warehouses = await _db.Warehouses.ToListAsync();
                return View();
            }

            _allocator.Occupy(warehouseId, placement.Value.x, placement.Value.y, placement.Value.z, wCells, dCells, hCells);

            var storageMonths = 1;

            if (plannedDate.HasValue && endDate.HasValue)
            {
                var start = plannedDate.Value.Date;
                var end = endDate.Value.Date;

                if (end > start)
                {
                    storageMonths = (int)Math.Ceiling((end - start).TotalDays / 30.0);
                }
            }
            else if (plannedDate.HasValue)
            {
                var today = DateTime.UtcNow.Date;
                var start = plannedDate.Value.Date;
                if (start > today)
                {
                    storageMonths = (int)Math.Ceiling((start - today).TotalDays / 30.0);
                }
            }

            var pricePerItem = CalculateStoragePricePerItem(weight, volume);
            var totalStoragePricePerMonth = pricePerItem * quantity;

            var mergedNotes = string.IsNullOrWhiteSpace(notes)
                ? description
                : $"{description}. {notes}";

            var order = new Order
            {
                UserAccountId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Awaiting,
                ServiceType = serviceType,
                WarehouseId = warehouseId,
                Quantity = quantity,
                WeightKg = weight,
                VolumeM3 = volume,
                StoragePricePerMonth = totalStoragePricePerMonth,
                StorageMonths = storageMonths,
                Notes = mergedNotes,
                IsPaid = false,
                StorageFrom = plannedDate?.Date,
                StorageTo = endDate?.Date
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            _db.Payments.Add(new Payment
            {
                OrderId = order.Id,
                Method = method,
                Status = PaymentStatus.Pending,
                Succeeded = false
            });

            await _db.SaveChangesAsync();

            // ==========================
            // ВІДПРАВКА ЛИСТА КОРИСТУВАЧУ
            // ==========================
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var totalPrice = totalStoragePricePerMonth * storageMonths;
                var subject = $"Створено замовлення №{order.Id}";

                var body = $@"
<h2>Ваше замовлення №{order.Id} успішно створено</h2>
<p><strong>Опис:</strong> {description}</p>
<p><strong>Склад:</strong> {warehouse.Name}</p>
<p><strong>Кількість:</strong> {quantity}</p>
<p><strong>Місяців зберігання:</strong> {storageMonths}</p>
<p><strong>Орієнтовна вартість зберігання:</strong> {totalPrice} грн</p>
<p>Статус оплати: очікує оплату.</p>
<p>Перейдіть в особистий кабінет, щоб завершити оплату.</p>
";

                await _emailService.SendAsync(userEmail, subject, body);
            }

            return RedirectToAction("Pay", "Payment", new { id = order.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _db.Orders
                .Include(o => o.Payment)
                .Include(o => o.Warehouse)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserAccountId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Extend(int id, int months)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _db.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserAccountId == userId);

            if (order == null) return NotFound();
            if (months <= 0)
            {
                ModelState.AddModelError(string.Empty, "Кількість місяців має бути більшою за 0.");
                return RedirectToAction(nameof(Details), new { id });
            }

            order.StorageMonths += months;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _db.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserAccountId == userId);

            if (order == null) return NotFound();
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Done)
                return BadRequest("Замовлення вже завершено і не може бути скасоване.");

            order.Status = OrderStatus.Canceled;
            if (order.Payment != null)
            {
                order.Payment.Status = PaymentStatus.Failed;
                order.Payment.Succeeded = false;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RetryPayment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _db.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserAccountId == userId);

            if (order == null) return NotFound();

            if (order.Payment == null)
            {
                order.Payment = new Payment
                {
                    OrderId = order.Id,
                    Method = PaymentMethod.Card,
                    Status = PaymentStatus.Pending,
                    Succeeded = false
                };
            }
            else
            {
                order.Payment.Status = PaymentStatus.Pending;
                order.Payment.Succeeded = false;
                order.Payment.PaidAt = null;
            }

            order.IsPaid = false;
            order.Status = OrderStatus.Awaiting;

            await _db.SaveChangesAsync();

            return RedirectToAction("Pay", "Payment", new { id = order.Id });
        }
    }
}
