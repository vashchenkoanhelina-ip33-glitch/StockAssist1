using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockAssist.Web.Data;
using StockAssist.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace StockAssist.Web.Services
{
    public class StorageReminderEmailService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<StorageReminderEmailService> _logger;

        public StorageReminderEmailService(
            IServiceProvider services,
            ILogger<StorageReminderEmailService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StorageReminderEmailService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        var now = DateTime.UtcNow.Date;

                        var candidates = await db.Orders
                            .Where(o =>
                                !o.IsPaid &&
                                o.StorageTo != null &&
                                !o.ReminderEmailSent &&
                                (o.Status == OrderStatus.Awaiting ||
                                 o.Status == OrderStatus.InProgress))
                            .ToListAsync(stoppingToken);

                        foreach (var order in candidates)
                        {
                            var storageFrom = order.StorageFrom?.Date ?? order.CreatedAt.Date;
                            var storageTo = order.StorageTo!.Value.Date;

                            var totalDays = (storageTo - storageFrom).TotalDays;
                            var daysLeft = (storageTo - now).TotalDays;

                            var shouldSend = false;

                            if (totalDays <= 4)
                            {
                                shouldSend = true;
                            }
                            else
                            {
                                if (daysLeft >= 0 && daysLeft <= 6)
                                    shouldSend = true;
                            }

                            if (!shouldSend)
                                continue;

                            var user = await userManager.FindByIdAsync(order.UserAccountId);
                            if (user == null || string.IsNullOrWhiteSpace(user.Email))
                                continue;

                            var subject = "Нагадування: скоро закінчується зберігання вашого замовлення";

                            var body = $@"
<p>Вітаємо, {user.UserName ?? user.Email}!</p>
<p>Нагадуємо, що <strong>через {Math.Max(0, (int)daysLeft)} днів</strong> закінчується термін зберігання вашого замовлення №{order.Id}.</p>
<p>Замовлення наразі <strong>ще не оплачено</strong>. Будь ласка, виконайте оплату, щоб ми продовжили зберігання без перерв.</p>
<p>Дані замовлення:</p>
<ul>
  <li>Склад: {order.WarehouseId}</li>
  <li>Період зберігання: {order.StorageFrom:dd.MM.yyyy} – {order.StorageTo:dd.MM.yyyy}</li>
  <li>Сума за місяць зберігання: {order.StoragePricePerMonth} грн</li>
</ul>
<p>Дякуємо, що користуєтесь нашим сервісом!</p>
";

                            try
                            {
                                await emailService.SendAsync(user.Email, subject, body);

                                order.ReminderEmailSent = true;
                                await db.SaveChangesAsync(stoppingToken);

                                _logger.LogInformation("Storage reminder email sent for order {OrderId}", order.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error sending storage reminder email for order {OrderId}", order.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "StorageReminderEmailService loop error");
                }
                await Task.Delay(TimeSpan.FromMinutes(100), stoppingToken);
            }

            _logger.LogInformation("StorageReminderEmailService stopping.");
        }
    }
}
