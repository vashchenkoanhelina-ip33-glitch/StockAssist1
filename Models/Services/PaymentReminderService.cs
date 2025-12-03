using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockAssist.Web.Data;
using StockAssist.Web.Models;

namespace StockAssist.Web.Services
{
    public class PaymentReminderService : BackgroundService
    {
        private readonly IServiceProvider sp;
        public PaymentReminderService(IServiceProvider s) => sp = s;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var mail = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var cutoff = DateTime.UtcNow.AddDays(-3);

                var unpaidOrders = await db.Orders
                    .Where(o => !o.IsPaid && o.CreatedAt <= cutoff)
                    .ToListAsync(stoppingToken);

                foreach (var o in unpaidOrders)
                {
                    var user = await db.UserAccounts.FindAsync(o.UserAccountId);

                    if (user?.Email != null)
                    {
                        var subject = $"Нагадування про оплату замовлення №{o.Id}";
                        var body = $@"
                            <p>Вітаємо, {user.FirstName ?? user.Email}!</p>
                            <p>Ваше замовлення <b>#{o.Id}</b> досі не оплачено.</p>
                            <p>Будь ласка, завершіть оплату найближчим часом.</p>";

                        await mail.SendAsync(user.Email, subject, body);
                    }
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}
