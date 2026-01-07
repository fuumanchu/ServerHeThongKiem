using MQTTnet;
using MQTTnet.Client;
using ServerHeThongKiem.Services.Interfaces;


namespace ServerHeThongKiem.Services
{
    public class MqttPublishService : IMqttPublishService
    {
        private readonly IMqttClient _client;
        private readonly ILogger<MqttPublishService> _logger;
        private readonly IDeviceCacheService _deviceCache;



        public MqttPublishService(IMqttClient client, ILogger<MqttPublishService> logger, IDeviceCacheService deviceCache)
        {
            _client = client;
            _logger = logger;
            _deviceCache = deviceCache;
        }

        public async Task PublishAsync(string topic, string payload)
        {
            if (_client == null) return; 
            if (!_client.IsConnected)
            {
                _logger.LogInformation("MQTT client is not connected. Attempting to reconnect...");
                throw new InvalidOperationException("MQTT not connected");
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
                
            await _client.PublishAsync(message);
            //thêm log để xem dữ liệu vừa gửi
            _logger.LogInformation($"Published to topic {topic}: {payload}");

        }
    }
}
