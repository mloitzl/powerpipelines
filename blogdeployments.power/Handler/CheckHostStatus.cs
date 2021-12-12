using System.Net.NetworkInformation;
using System.Text;
using blogdeployments.power.Service;
using MediatR;

namespace blogdeployments.power.Handler;

public class CheckHostStatus : IRequest<bool>
{
    public string Hostname { get; set; }

    // fixme: return bool not necessary
    public class CheckHostStatusHandler : IRequestHandler<CheckHostStatus, bool>
    {
        private readonly ILogger<CheckHostStatusHandler> _logger;
        private readonly IRaspbeeService _raspbeeService;

        public CheckHostStatusHandler(IRaspbeeService raspbeeService, ILogger<CheckHostStatusHandler> logger)
        {
            _raspbeeService = raspbeeService;
            _logger = logger;
        }

        public Task<bool> Handle(CheckHostStatus request, CancellationToken cancellationToken)
        {
            // todo: Wait for ping to fail
            var who = request.Hostname;
            var pingSender = new Ping();
            var waiter = new AutoResetEvent(false);

            pingSender.PingCompleted += (sender, e) =>
            {
                if (e.Cancelled)
                {
                    Console.WriteLine("Ping canceled.");

                    // Let the main thread resume.
                    // UserToken is the AutoResetEvent object that the main thread
                    // is waiting for.  
                    ((AutoResetEvent) e.UserState).Set();
                }

                // If an error occurred, display the exception to the user.  
                if (e.Error != null)
                {
                    Console.WriteLine("Ping failed:");
                    Console.WriteLine(e.Error.ToString());

                    // Let the main thread resume.
                    ((AutoResetEvent) e.UserState).Set();
                }

                var reply = e.Reply;

                LogReply(reply);

                // Let the main thread resume.  
                ((AutoResetEvent) e.UserState).Set();
            };
            var data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            var buffer = Encoding.ASCII.GetBytes(data);

            // Wait 12 seconds for a reply.  
            var timeout = 12000;

            // Set options for transmission:  
            // The data can go through 64 gateways or routers  
            // before it is destroyed, and the data packet  
            // cannot be fragmented.  
            var options = new PingOptions(64, true);

            Console.WriteLine("Time to live: {0}", options.Ttl);
            Console.WriteLine("Don't fragment: {0}", options.DontFragment);

            // Send the ping asynchronously.  
            // Use the waiter as the user token.  
            // When the callback completes, it can wake up this thread.  
            pingSender.SendAsync(who, timeout, buffer, options, waiter);

            // Prevent this example application from ending.  
            // A real application should do something useful  
            // when possible.  
            waiter.WaitOne();

            _logger.LogDebug(request.Hostname);

            return Task.FromResult(true);
        }

        private void LogReply(PingReply reply)
        {
            if (reply == null)
                return;

            _logger.LogDebug("ping status: {0}", reply.Status);
            if (reply.Status == IPStatus.Success)
            {
                _logger.LogDebug("Address: {0}", reply.Address.ToString());
                _logger.LogDebug("RoundTrip time: {0}", reply.RoundtripTime);
                _logger.LogDebug("Time to live: {0}", reply.Options.Ttl);
                _logger.LogDebug("Don't fragment: {0}", reply.Options.DontFragment);
                _logger.LogDebug("Buffer size: {0}", reply.Buffer.Length);
            }
        }
    }
}