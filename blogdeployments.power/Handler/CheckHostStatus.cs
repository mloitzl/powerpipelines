using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.power.Service;
using blogdeployments.repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace blogdeployments.power.Handler;

public class CheckHostStatus : IRequest<bool>
{
    public string Hostname { get; set; }
    public DateTime InitiatedTime { get; set; }

    // fixme: return bool not necessary
    public class CheckHostStatusHandler : IRequestHandler<CheckHostStatus, bool>
    {
        private readonly IClusterPowerStatusRepository _clusterPowerStatusRepository;
        private readonly IOptions<ClusterTopologyConfiguration> _configuration;
        private readonly IEventSender<ShutdownInitiated> _eventSender;
        private readonly ILogger<CheckHostStatusHandler> _logger;
        private readonly IRaspbeeService _raspbeeService;

        public CheckHostStatusHandler(
            IEventSender<ShutdownInitiated> eventSender,
            IOptions<ClusterTopologyConfiguration> configuration,
            IClusterPowerStatusRepository clusterPowerStatusRepository,
            IRaspbeeService raspbeeService,
            ILogger<CheckHostStatusHandler> logger)
        {
            _eventSender = eventSender;
            _configuration = configuration;
            _clusterPowerStatusRepository = clusterPowerStatusRepository;
            _raspbeeService = raspbeeService;
            _logger = logger;
        }

        public async Task<bool> Handle(CheckHostStatus request, CancellationToken cancellationToken)
        {
            Debug.Assert(request != null, nameof(request) + " != null");
            _logger.LogDebug("{Hostname} {CheckingSince} s", request?.Hostname,
                DateTime.Now.Subtract(request.InitiatedTime).TotalSeconds);

            var who = request.Hostname;
            var pingSender = new Ping();
            var waiter = new AutoResetEvent(false);

            pingSender.PingCompleted += async (sender, e) =>
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

                if (reply != null && reply.Status == IPStatus.TimedOut)
                {
                    // set host status to off
                    await _clusterPowerStatusRepository.EnsureHostPowerStatus(_configuration.Value.ClusterId,
                        request.Hostname, new HostPowerStatus
                        {
                            Hostname = request.Hostname,
                            Status = PowerStatus.Off
                        });

                    var clusterPowerStatus =
                        await _clusterPowerStatusRepository.GetPowerStatus(_configuration.Value.ClusterId);

                    var sb = clusterPowerStatus.HostsPower.Aggregate(new StringBuilder(), (builder, pair) =>
                    {
                        builder.Append($"{pair.Key}: {pair.Value.Status} | ");
                        return builder;
                    });

                    if (sb != null) _logger.LogDebug(sb.ToString());

                    // check if we can turn off (all hosts off)
                    if (clusterPowerStatus.HostsPower.All(kvp => kvp.Value.Status == PowerStatus.Off))
                        _raspbeeService.PowerOff();
                }

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

            // _logger.LogDebug("Time to live: {Ttl}", options.Ttl);
            // _logger.LogDebug("Don't fragment: {DontFragment}", options.DontFragment);

            // Send the ping asynchronously.  
            // Use the waiter as the user token.  
            // When the callback completes, it can wake up this thread.  
            pingSender.SendAsync(who, timeout, buffer, options, waiter);

            // Prevent this example application from ending. 
            // A real application should do something useful  
            // when possible.  
            waiter.WaitOne();

            // _logger.LogDebug("WaitOne returned, {Hostname}", request?.Hostname);

            return true;
        }

        private void LogReply(PingReply reply)
        {
            if (reply == null)
                return;

            _logger.LogDebug("ping status: {Status}", reply.Status);
            // if (reply.Status == IPStatus.Success)
            // {
            //     _logger.LogDebug("Address: {Address}", reply.Address.ToString());
            //     _logger.LogDebug("RoundTrip time: {RoundTripTime}", reply.RoundtripTime);
            //     _logger.LogDebug("Time to live: {Ttl}", reply?.Options?.Ttl);
            //     _logger.LogDebug("Don't fragment: {DontFragment}", reply?.Options?.DontFragment);
            //     _logger.LogDebug("Buffer size: {BufferLength}", reply?.Buffer?.Length);
            // }
        }
    }
}