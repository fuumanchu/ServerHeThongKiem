using Microsoft.EntityFrameworkCore;
using ServerHeThongKiem.Services.Interfaces;
using System;

namespace ServerHeThongKiem.Services
{
    public class DeviceCacheService : IDeviceCacheService
    {
        private readonly HashSet<string> _deviceIds = new();
        private readonly IServiceScopeFactory _scopeFactory; 

        public DeviceCacheService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public bool ExistsDeviceID(string deviceId)
            => _deviceIds.Contains(deviceId);

        public void AddDeviceID(string deviceID)
            => _deviceIds.Add(deviceID);

        public void RemoveDeviceID(string deviceID)
            => _deviceIds.Remove(deviceID);

        public IReadOnlyCollection<string> GetAllDeviceID()
            => _deviceIds;

        public async Task ReloadFromDatabaseAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var ids = await db.Devices.Select(d => d.DeviceID).ToListAsync();

            _deviceIds.Clear();
            foreach (var id in ids)
                _deviceIds.Add(id);
        }
    }
}
