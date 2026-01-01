using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client; 
using ServerHeThongKiem.Models;
using ServerHeThongKiem.Services;
using System.Text;

namespace ServerHeThongKiem.Services
{
    public class MqttWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MqttWorkerService> _logger;
        private IMqttClient _mqttClient;

        public MqttWorkerService(IServiceScopeFactory scopeFactory, ILogger<MqttWorkerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithCleanSession()
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                // Sửa lỗi PayloadSegment thành Payload
                var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var topic = e.ApplicationMessage.Topic;

                return HandleMessage(topic, payload);
            };

            // Logic kết nối lại khi mất mạng
            _mqttClient.DisconnectedAsync += async e =>
            {
                _logger.LogWarning("Mất kết nối MQTT. Đang thử kết nối lại...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try
                {
                    await _mqttClient.ConnectAsync(options, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Không thể kết nối lại: {ex.Message}");
                }
            };

            await _mqttClient.ConnectAsync(options, stoppingToken);

            await _mqttClient.SubscribeAsync("Devices/+/Data");

            _logger.LogInformation("MQTT Worker đã kết nối và đang lắng nghe...");

            // Giữ service chạy cho đến khi bị stop
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleMessage(string topic, string payload)
        {
            var topicParts = topic.Split('/');
            if (topicParts.Length < 2) return;

            string deviceId = topicParts[1];

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var device = await context.Devices
                    .Include(d => d.Inputs)
                    .FirstOrDefaultAsync(d => d.DeviceID == deviceId);

                if (device != null)
                {
                    var tdsInput = device.Inputs.FirstOrDefault(i => i.Name == "CB TDS 1");
                    if (tdsInput != null)
                    {
                        tdsInput.Value = payload;
                        device.Status = "Online";
                        await context.SaveChangesAsync();
                        _logger.LogInformation($"Đã cập nhật TDS cho {deviceId}: {payload}");
                    }
                }
            }
        }
    } 
} 