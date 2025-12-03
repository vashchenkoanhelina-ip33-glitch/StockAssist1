using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockAssist.Web.Data;
using StockAssist.Web.Models;

namespace StockAssist.Web.Services
{
    public class OrderStatusUpdateService : BackgroundService
    {
        private readonly IServiceProvider _sp;

        public OrderStatusUpdateService(IServiceProvider sp)
        {
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var today = DateTime.UtcNow.Date;

                    var orders = await db.Orders
                        .Where(o => o.Status != OrderStatus.Canceled &&
                                    o.Status != OrderStatus.Done)
                        .ToListAsync(stoppingToken);

                    foreach (var o in orders)
                    {
                        if (!o.IsPaid)
                        {
                            o.Status = OrderStatus.Awaiting;
                        }
                        else
                        {
                            var start = (o.StorageFrom ?? o.CreatedAt).Date;
                            DateTime end;

                            if (o.StorageTo.HasValue)
                            {
                                end = o.StorageTo.Value.Date;
                            }
                            else
                            {
                                var months = o.StorageMonths > 0 ? o.StorageMonths : 1;
                                end = start.AddMonths(months);
                            }

                            if (today < start)
                            {
                                o.Status = OrderStatus.Awaiting;
                            }
                            else if (today >= start && today <= end)
                            {
                                o.Status = OrderStatus.InProgress;
                            }
                            else if (today > end)
                            {
                                o.Status = OrderStatus.Completed;
                            }
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[OrderStatusUpdateService] Error: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
