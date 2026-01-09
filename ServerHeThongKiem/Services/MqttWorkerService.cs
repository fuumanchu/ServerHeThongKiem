using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using ServerHeThongKiem.Services.Interfaces;
using System.Text;

namespace ServerHeThongKiem.Services
{
    public class MqttWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MqttWorkerService> _logger;
        private readonly IMqttClient _mqttClient;
        private readonly IDeviceCacheService _deviceCache;
        private readonly IHubContext<DeviceHub> _hubContext;

        public MqttWorkerService(
            IServiceScopeFactory scopeFactory,
            ILogger<MqttWorkerService> logger,
            IDeviceCacheService deviceCache,
            IMqttClient mqttClient,
            IHubContext<DeviceHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _deviceCache = deviceCache;
            _mqttClient = mqttClient;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _deviceCache.ReloadFromDatabaseAsync();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithCleanSession()
                .Build();

            // Nhận message
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                // Bỏ retained message
                if (e.ApplicationMessage.Retain)
                {
                    _logger.LogWarning($"Bỏ qua retained message: {e.ApplicationMessage.Topic}");
                    return Task.CompletedTask;
                }

                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var topic = e.ApplicationMessage.Topic;

                return HandleMessage(topic, payload);
            };

            // Reconnect
            _mqttClient.DisconnectedAsync += async e =>
            {
                _logger.LogWarning("Mất kết nối MQTT, đang reconnect...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

                try
                {
                    if (!_mqttClient.IsConnected)
                        await _mqttClient.ConnectAsync(options, stoppingToken);

                    await _mqttClient.SubscribeAsync("Devices/+/Data");
                    _logger.LogInformation("Reconnect MQTT thành công & subscribe lại.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reconnect MQTT thất bại");
                }
            };

            // Connect + Subscribe
            await _mqttClient.ConnectAsync(options, stoppingToken);
            await _mqttClient.SubscribeAsync("Devices/+/Data");

            _logger.LogInformation("MQTT Worker đã kết nối & lắng nghe Devices/+/Data");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleMessage(string topic, string payload)
        {
            // Topic dạng: Devices/{deviceId}/Data
            var topicParts = topic.Split('/');
            if (topicParts.Length < 3) return;

            string deviceId = topicParts[1];

            if (!_deviceCache.ExistsDeviceID(deviceId))
            {
                _logger.LogWarning($"Thiết bị chưa đăng ký: {deviceId}");
                return;
            }

            // Parse payload: type|order|value
            var parts = payload.Split('|');
            if (parts.Length < 3)
            {
                _logger.LogWarning($"Payload sai format: {payload}");
                return;
            }

            string type = parts[0];        // Input / Output
            string orderStr = parts[1];
            string value = parts[2];

            _logger.LogInformation($"MQTT {deviceId}: {payload}");
            _logger.LogWarning($"CHECK ONLINE: type={type}, order={orderStr}, value={value}");

            // ✅ ONLINE LOGIC: chỉ khi mô phỏng cảm biến (Input) thì mới update LastSeen
            // (Không cần parse value -> ON/OFF vẫn tính online)
            if (string.Equals(type, "Input", StringComparison.OrdinalIgnoreCase))
            {
                // ✅ ONLINE: cứ có message từ device là coi Online
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var dev = await db.Devices.FirstOrDefaultAsync(d => d.DeviceID == deviceId);
                    if (dev != null)
                    {
                        dev.LastSeen = DateTime.Now;

                        // ✅ thêm dòng này
                        if (dev.Status != "Online")
                            dev.Status = "Online";

                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Update LastSeen/Status lỗi cho {deviceId}");
                }
            }

                // ✅ SignalR: vẫn gửi toàn bộ dữ liệu
                await _hubContext.Clients.All.SendAsync("NotifyUpdate", new
            {
                deviceId,
                type,
                order = orderStr,
                value
            });
        }

        // ✅ Publish command
        public async Task PublishMessageAsync(string deviceId, string payload)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                _logger.LogWarning("MQTT chưa kết nối");
                return;
            }

            if (!_deviceCache.ExistsDeviceID(deviceId))
            {
                _logger.LogWarning($"Thiết bị chưa đăng ký: {deviceId}");
                return;
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"Devices/{deviceId}/Command")
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(message);
            _logger.LogInformation($"Đã gửi lệnh tới {deviceId}: {payload}");
        }
    }
}
