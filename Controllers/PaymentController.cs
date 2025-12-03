using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAssist.Web.Data;
using StockAssist.Web.Models;

namespace StockAssist.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Pay(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Warehouse)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge(); 
            var appUser = await _userManager.FindByIdAsync(userId);


            var vm = new PayViewModel
            {
                OrderId = order.Id,
                OrderNumber = order.Id,
                StoragePricePerMonth = order.StoragePricePerMonth,
                StorageMonths = order.StorageMonths,
                TotalAmount = order.StoragePricePerMonth * order.StorageMonths,
                
                SavedCardLast4 = appUser?.SavedCardLast4,
                SavedCardBrand = appUser?.SavedCardBrand,
                SavedCardExpiry = appUser?.SavedCardExpiry,

                Phone = appUser?.PhoneNumber
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PayViewModel model)
        {
            ModelState.Remove(nameof(PayViewModel.CardExpiry));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = await _db.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
                return NotFound();

            var totalAmount = order.StoragePricePerMonth * order.StorageMonths;

            Payment payment;
            if (order.Payment == null)
            {
                payment = new Payment
                {
                    OrderId = order.Id
                };
                _db.Payments.Add(payment);
                order.Payment = payment;
            }
            else
            {
                payment = order.Payment;
            }

            payment.Method = PaymentMethod.Card;
            payment.Amount = totalAmount;
            payment.Status = PaymentStatus.Paid;
            payment.Succeeded = true;
            payment.PaidAt = DateTime.UtcNow;

            order.IsPaid = true;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge(); 
            var appUser = await _userManager.FindByIdAsync(userId);


            if (appUser != null && model.SaveCard)
            {
                if (!string.IsNullOrWhiteSpace(model.CardNumber) && model.CardNumber.Length >= 4)
                {
                    appUser.SavedCardLast4 = model.CardNumber[^4..];
                }

                appUser.SavedCardBrand = !string.IsNullOrWhiteSpace(model.CardBrand)
                    ? model.CardBrand
                    : model.PaymentMethod;

                appUser.SavedCardExpiry = model.CardExpiry;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayWithSavedCard(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            var totalAmount = order.StoragePricePerMonth * order.StorageMonths;

            if (order.Payment == null)
            {
                var payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = totalAmount,
                    Method = PaymentMethod.Card,
                    Status = PaymentStatus.Paid,
                    Succeeded = true,
                    PaidAt = DateTime.UtcNow
                };

                _db.Payments.Add(payment);
                order.Payment = payment;
            }
            else
            {
                order.Payment.Amount = totalAmount;
                order.Payment.Status = PaymentStatus.Paid;
                order.Payment.Succeeded = true;
                order.Payment.PaidAt = DateTime.UtcNow;
            }

            order.IsPaid = true;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }
    }
}
