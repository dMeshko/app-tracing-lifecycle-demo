using System.Diagnostics;
using MassTransit;

namespace AppiLifecycleDemo.Observers
{
    public class ConsumeObserver : IConsumeObserver
    {
        private Activity _activity;

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            _activity = new Activity($"Sender:{context.SourceAddress}-Consumer:{context.DestinationAddress}");
            if (context.TryGetHeader<string>("traceparent", out var traceParent))
            {
                _activity.SetParentId(traceParent);
                if (context.TryGetHeader<string>("tracestate", out var traceState))
                {
                    _activity.TraceStateString = traceState;
                    //_activity.AddBaggage("tracestate", traceState);
                }
            }
            _activity.Start();

            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            if (_activity.Duration == TimeSpan.Zero)
            {
                _activity.SetEndTime(DateTime.UtcNow);
            }

            _activity.Stop();
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
