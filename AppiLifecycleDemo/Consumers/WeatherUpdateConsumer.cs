using System.Diagnostics;
using AppiLifecycleDemo.Contracts;
using MassTransit;

namespace AppiLifecycleDemo.Consumers
{
    public class WeatherUpdateConsumer : IConsumer<WeatherUpdate>
    {
        private readonly ILogger<WeatherUpdateConsumer> _logger;

        public WeatherUpdateConsumer(ILogger<WeatherUpdateConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<WeatherUpdate> context)
        {
            _logger.LogInformation("[{Consumer}] Received weather update: {temperature}°C", nameof(WeatherUpdateConsumer), context.Message.Temperature);
            _logger.LogWarning("{CurrentActivityId} <-> {ParentActivityId}", Activity.Current.Id, Activity.Current.ParentId);
            return Task.CompletedTask;
        }
    }
}
