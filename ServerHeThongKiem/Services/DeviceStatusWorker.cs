using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServerHeThongKiem.Services
{
    public class DeviceStatusWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // ✅ bạn chỉnh ngưỡng ở đây: 2 giây là "tắt gần như offline liền"
        private static readonly TimeSpan OfflineAfter = TimeSpan.FromSeconds(2);

        public DeviceStatusWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var now = DateTime.Now;
                    var threshold = now - OfflineAfter;

                    // ✅ Mark Offline rất nhanh nếu quá hạn
                    var toOffline = await db.Devices
                        .Where(d => d.Status == "Online"
                                    && (d.LastSeen == null || d.LastSeen < threshold))
                        .ToListAsync(stoppingToken);

                    if (toOffline.Count > 0)
                    {
                        foreach (var d in toOffline)
                            d.Status = "Offline";

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch
                {
                    // tránh crash service; bạn có thể log nếu muốn
                }

                // ✅ quét mỗi 1 giây
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
