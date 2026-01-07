using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client; 
using ServerHeThongKiem.Models;
using ServerHeThongKiem.Services;
using ServerHeThongKiem.Services.Interfaces;
using System.Text;
using System.Windows; 

namespace ServerHeThongKiem.Services
{
    public class MqttWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MqttWorkerService> _logger;
        private readonly IMqttClient _mqttClient;
        private readonly IDeviceCacheService _deviceCache;
        private readonly IHubContext<DeviceHub> _hubContext;
        public static IMqttClient Client { get; private set; } = null!;
        public MqttWorkerService(IServiceScopeFactory scopeFactory, ILogger<MqttWorkerService> logger, IDeviceCacheService deviceCache, IMqttClient mqttClient, IHubContext<DeviceHub> hubContext)
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

            if(!_deviceCache.ExistsDeviceID(deviceId))
            {
                _logger.LogWarning($"Nhận dữ liệu từ thiết bị chưa đăng ký: {deviceId}");
                return;
            }
            else
            {
                //hiện message dữ liệu nhận được để test
                _logger.LogInformation($"Dữ liệu từ {deviceId}: {payload}");
                //hithông báo đã nhận dữ liệu từ thiết bị đã đăng ký
                _logger.LogInformation($"Đã nhận dữ liệu từ thiết bị đã đăng ký: {deviceId}");
                //hiện thị dữ liệu kiểu message box 
                var parts = payload.Split('|');
                var updateData = new
                {
                    deviceId = deviceId,
                    type = parts[0],  // "Input" hoặc "Output"
                    order = parts[1], // STT (1-23)
                    value = parts[2]  // Giá trị đo được
                };
                await _hubContext.Clients.All.SendAsync("NotifyUpdate", updateData);

            }
        }


        public async Task PublishMessageAsync(string topic, string payload)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                _logger.LogWarning("MQTT client chưa kết nối. Không thể gửi tin nhắn.");
                return;
            }
            if(!_deviceCache.ExistsDeviceID(topic.Split('/')[1]))
            {
                _logger.LogWarning($"Không thể gửi tin nhắn đến thiết bị chưa đăng ký: {topic}");
                return;
            }
            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"Devices/{topic}/Command")
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            _logger.LogInformation($"Đã gửi lệnh tới {topic}: {payload}");

        }
    } 
} 