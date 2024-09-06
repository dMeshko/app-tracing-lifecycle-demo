using System.Diagnostics;
using AppiLifecycleDemo.Contracts;
using MassTransit;

namespace AppiLifecycleDemo.Filters
{
    public class PublishFilter<T> : IFilter<PublishContext<T>> where T : BaseContract
    {
        private readonly ILogger<PublishFilter<T>> _logger;

        public PublishFilter(ILogger<PublishFilter<T>> logger)
        {
            _logger = logger;
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            var activity = new Activity($"Sender:{context.SourceAddress}-Consumer:{context.DestinationAddress}");
            activity.Start();

            if (!context.TryGetHeader<string>("traceparent", out var traceParent))
            {
                context.Headers.Set("traceparent", activity.Id);
                if (activity.TraceStateString != null)
                {
                    context.Headers.Set("tracestate", activity.TraceStateString);
                }
            }

            try
            {
                next.Send(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            finally
            {
                if (activity.Duration == TimeSpan.Zero)
                {
                    activity.SetEndTime(DateTime.UtcNow);
                }

                activity.Stop();
            }

            return Task.CompletedTask;
        }

        public void Probe(ProbeContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
