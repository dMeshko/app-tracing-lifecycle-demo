using System.Diagnostics;
using AppiLifecycleDemo.Contracts;
using MassTransit;

namespace AppiLifecycleDemo.Consumers
{
    public class SaySomethingConsumer2 : IConsumer<SaySomething>
    {
        private readonly ILogger<SaySomethingConsumer2> _logger;

        public SaySomethingConsumer2(ILogger<SaySomethingConsumer2> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SaySomething> context)
        {
            _logger.LogInformation("[{Consumer}] Received message: {message}", nameof(SaySomethingConsumer2), context.Message.Message);
            _logger.LogWarning("{CurrentActivityId} <-> {ParentActivityId}", Activity.Current.Id, Activity.Current.ParentId);
            return Task.CompletedTask;
        }
    }
}
