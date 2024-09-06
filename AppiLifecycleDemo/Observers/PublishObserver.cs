using System.Diagnostics;
using MassTransit;

namespace AppiLifecycleDemo.Observers
{
    public class PublishObserver : IPublishObserver
    {
        private Activity _activity;

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            _activity = new Activity($"Sender:{context.SourceAddress}-Consumer:{context.DestinationAddress}");
            _activity.Start();

            if (!context.TryGetHeader<string>("traceparent", out var traceParent))
            {
                context.Headers.Set("traceparent", _activity.Id);
                if (_activity.TraceStateString != null)
                {
                    context.Headers.Set("tracestate", _activity.TraceStateString);
                }
            }

            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            if (_activity.Duration == TimeSpan.Zero)
            {
                _activity.SetEndTime(DateTime.UtcNow);
            }

            _activity.Stop();
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
