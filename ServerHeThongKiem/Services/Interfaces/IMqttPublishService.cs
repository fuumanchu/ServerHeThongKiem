namespace ServerHeThongKiem.Services.Interfaces
{
    public interface IMqttPublishService
    {
        Task PublishAsync(string topic, string payload);
    }
}
