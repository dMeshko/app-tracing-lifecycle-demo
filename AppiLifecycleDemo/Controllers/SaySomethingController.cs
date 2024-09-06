using System.Diagnostics;
using AppiLifecycleDemo.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace AppiLifecycleDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SaySomethingController : ControllerBase
    {
        private readonly ILogger<SaySomethingController> _logger;
        private readonly IBus _bus;

        public SaySomethingController(ILogger<SaySomethingController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpGet]
        public async Task<ActionResult> Get(string message)
        {
            var command = new SaySomething(message);

            _logger.LogInformation("Sending message {message}", command.Message);
            _logger.LogWarning("{CurrentActivityId} <-> {ParentActivityId}", Activity.Current.Id, Activity.Current.ParentId);

            await _bus.Publish(command);

            return Accepted();
        }
    }
}
