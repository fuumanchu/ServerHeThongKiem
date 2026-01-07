namespace ServerHeThongKiem.Services.Interfaces
{
    public interface IDeviceCacheService
    {
        bool ExistsDeviceID(string deviceId);
        void AddDeviceID(string deviceID); 
        void RemoveDeviceID(string deviceID);
        IReadOnlyCollection<string> GetAllDeviceID();

        Task ReloadFromDatabaseAsync();
    }
}
