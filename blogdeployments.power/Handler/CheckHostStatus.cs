using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using MediatR;

namespace blogdeployments.power.Handler;

public class CheckHostStatus : IRequest<bool>
{
    public string Hostname { get; set; }
    public DateTime InitiatedTime { get; set; }

    // fixme: return bool not necessary
    public class CheckHostStatusHandler : IRequestHandler<CheckHostStatus, bool>
    {
        private readonly IEventSender<ShutdownInitiated> _eventSender;
        private readonly ILogger<CheckHostStatusHandler> _logger;

        public CheckHostStatusHandler(
            IEventSender<ShutdownInitiated> eventSender,
            ILogger<CheckHostStatusHandler> logger)
        {
            _eventSender = eventSender;
            _logger = logger;
        }

        public Task<bool> Handle(CheckHostStatus request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{Hostname}", request?.Hostname);
            Debug.Assert(request != null, nameof(request) + " != null");
            _logger.LogDebug("{CheckingSince} s", DateTime.Now.Subtract(request.InitiatedTime).TotalMilliseconds);
            var who = request.Hostname;
            var pingSender = new Ping();
            var waiter = new AutoResetEvent(false);

            pingSender.PingCompleted += (sender, e) =>
            {
                if (e.Cancelled)
                {
                    _logger.LogDebug("Ping canceled");

                    // Let the main thread resume.
                    // UserToken is the AutoResetEvent object that the main thread
                    // is waiting for.  
                    ((AutoResetEvent) e.UserState).Set();
                }

                // If an error occurred, display the exception to the user.  
                if (e.Error != null)
                {
                    _logger.LogDebug("Ping failed:");
                    _logger.LogDebug(e.Error.ToString());

                    // Let the main thread resume.
                    ((AutoResetEvent) e.UserState).Set();
                }

                var reply = e.Reply;

                LogReply(reply);

                if (reply != null && reply.Status == IPStatus.Success)
                {
                    Thread.Sleep(1000);
                    _eventSender.Send(new ShutdownInitiated
                    {
                        Hostname = who,
                        InitiatedTime = request.InitiatedTime
                    });
                }

                // Let the main thread resume.  
                ((AutoResetEvent) e.UserState).Set();
            };
            
            var data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            var buffer = Encoding.ASCII.GetBytes(data);

            // Wait 1 second for a reply.  
            var timeout = 1000;

            // Set options for transmission:  
            // The data can go through 64 gateways or routers  
            // before it is destroyed, and the data packet  
            // cannot be fragmented.  
            var options = new PingOptions(64, true);

            _logger.LogDebug("Time to live: {Ttl}", options.Ttl);
            _logger.LogDebug("Don't fragment: {DontFragment}", options.DontFragment);

            // Send the ping asynchronously.  
            // Use the waiter as the user token.  
            // When the callback completes, it can wake up this thread.  
            pingSender.SendAsync(who, timeout, buffer, options, waiter);

            // Prevent this example application from ending. 
            // A real application should do something useful  
            // when possible.  
            waiter.WaitOne();

            _logger.LogDebug("WaitOne returned, {Hostname}", request?.Hostname);

            return Task.FromResult(true);
        }

        private void LogReply(PingReply reply)
        {
            if (reply == null)
                return;

            _logger.LogDebug("ping status: {Status}", reply.Status);
            if (reply.Status == IPStatus.Success)
            {
                _logger.LogDebug("Address: {Address}", reply.Address.ToString());
                _logger.LogDebug("RoundTrip time: {RoundTripTime}", reply.RoundtripTime);
                _logger.LogDebug("Time to live: {Ttl}", reply?.Options?.Ttl);
                _logger.LogDebug("Don't fragment: {DontFragment}", reply?.Options?.DontFragment);
                _logger.LogDebug("Buffer size: {BufferLength}", reply?.Buffer?.Length);
            }
        }
    }
}