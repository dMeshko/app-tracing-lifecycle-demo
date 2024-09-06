using System.Diagnostics;
using AppiLifecycleDemo.Contracts;
using MassTransit;

namespace AppiLifecycleDemo.Consumers
{
    public class SaySomethingConsumer : IConsumer<SaySomething>
    {
        private readonly ILogger<SaySomethingConsumer> _logger;
        private readonly HttpClient _httpClient;

        public SaySomethingConsumer(ILogger<SaySomethingConsumer> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }


        public async Task Consume(ConsumeContext<SaySomething> context)
        {
            var forecast = await _httpClient.GetFromJsonAsync<IList<WeatherForecast>>("WeatherForecast");
            var sample = forecast!.First();

            var temp = sample.TemperatureC;
            _logger.LogInformation("[{Consumer}] Received message: {message}. Sending weather update..", nameof(SaySomethingConsumer), context.Message.Message);
            _logger.LogWarning("{CurrentActivityId} <-> {ParentActivityId}", Activity.Current.Id, Activity.Current.ParentId);

            await context.Publish(new WeatherUpdate(temp));
        }
    }
}
