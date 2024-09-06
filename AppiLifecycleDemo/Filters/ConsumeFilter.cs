using System.Diagnostics;
using AppiLifecycleDemo.Contracts;
using MassTransit;

namespace AppiLifecycleDemo.Filters
{
    public class ConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : BaseContract
    {
        private readonly ILogger<ConsumeFilter<T>> _logger;

        public ConsumeFilter(ILogger<ConsumeFilter<T>> logger)
        {
            _logger = logger;
        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var activity = new Activity($"Sender:{context.SourceAddress}-Consumer:{context.DestinationAddress}");
            if (context.TryGetHeader<string>("traceparent", out var traceParent))
            {
                activity.SetParentId(traceParent);
                if (context.TryGetHeader<string>("tracestate", out var traceState))
                {
                    activity.TraceStateString = traceState;
                    //_activity.AddBaggage("tracestate", traceState);
                }
            }
            activity.Start();

            try
            {
                next.Send(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
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
