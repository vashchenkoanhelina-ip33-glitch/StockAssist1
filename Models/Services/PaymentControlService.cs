using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockAssist.Web.Data;
using StockAssist.Web.Models;

namespace StockAssist.Web.Services
{
    public class PaymentControlService : BackgroundService
    {
        private readonly IServiceProvider sp;
        public PaymentControlService(IServiceProvider s) => sp = s;

        private DateTime GetNextPaymentDate(UserAccount user)
        {
            var last = user.LastPaymentDate ?? DateTime.UtcNow;

            return user.PaymentFrequency?.ToLower() switch
            {
                "monthly" => last.AddMonths(1),
                "quarterly" => last.AddMonths(3),
                "semiannual" => last.AddMonths(6),
                "yearly" => last.AddYears(1),
                _ => last.AddMonths(1) 
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var mail = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var users = await db.UserAccounts.ToListAsync(stoppingToken);

                foreach (var u in users)
                {
                    var nextPayment = GetNextPaymentDate(u);
                    if (DateTime.UtcNow >= nextPayment)
                    {
                        var subject = "Нагадування про оплату";
                        var body = $@"
                            <p>Вітаємо, {u.FirstName ?? u.Email}!</p>
                            <p>Настав час оплати вашого плану <b>{u.PaymentFrequency ?? "місячного"}</b>.</p>
                            <p>Будь ласка, увійдіть у систему для підтвердження платежу.</p>";

                        await mail.SendAsync(u.Email!, subject, body);

                        u.LastPaymentDate = DateTime.UtcNow;
                    }
                }

                await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}
