using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAssist.Web.Data;
using StockAssist.Web.Models;
using StockAssist.Web.Services;

namespace StockAssist.Web.Controllers
{
    [Authorize(Roles = "Operator")]
    public class OperatorController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly GmailEmailService _emailService;

        public OperatorController(ApplicationDbContext db, GmailEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }
        public async Task<IActionResult> Queue()
        {
            var orders = await _db.Orders
                .Include(o => o.Warehouse)
                .Include(o => o.Payment)
                .Where(o => o.Status == OrderStatus.InProgress)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.Done;
            await _db.SaveChangesAsync();

            await NotifyCustomerAsync(
                order,
                $"Ваше замовлення №{order.Id} виконано",
                $"Ваше замовлення №{order.Id} було успішно виконано. Дякуємо, що користуєтесь сервісом StockAssist."
            );

            TempData["SuccessMessage"] = $"Замовлення №{order.Id} позначено як виконане.";
            return RedirectToAction(nameof(Queue));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var order = await _db.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.Canceled;

            if (!string.IsNullOrWhiteSpace(reason))
            {
                var note = $"Відмова оператора: {reason}";
                order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                    ? note
                    : order.Notes + "\n" + note;
            }

            await _db.SaveChangesAsync();

            await NotifyCustomerAsync(
                order,
                $"Ваше замовлення №{order.Id} відхилено",
                $"Ваше замовлення №{order.Id} було відхилено. Причина: {reason}"
            );

            TempData["SuccessMessage"] = $"Замовлення №{order.Id} відхилено.";
            return RedirectToAction(nameof(Queue));
        }
        
        private async Task NotifyCustomerAsync(Order order, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(order.UserAccountId))
                    return;

                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == order.UserAccountId);

                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                    return;

                await _emailService.SendAsync(user.Email, subject, body);
            }
            catch
            {
            }
        }
    }
}
