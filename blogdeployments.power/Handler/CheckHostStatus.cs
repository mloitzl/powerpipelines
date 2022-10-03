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
        private readonly ClusterTopologyConfiguration _clusterConfiguration;
        private readonly IEventSender<ShutdownInitiated> _shutdownInitiatedEventSender;
        private readonly IEventSender<ShutdownCompleted> _shutdownCompletedEventSender;
        private readonly ILogger<CheckHostStatusHandler> _logger;
        private readonly IRaspbeeService _raspbeeService;

        public CheckHostStatusHandler(
            IEventSender<ShutdownInitiated> shutdownInitiatedShutdownInitiatedEventSender,
            IEventSender<ShutdownCompleted> shutdownCompletedEventSender,
            IOptions<ClusterTopologyConfiguration> clusterConfiguration,
            IOptions<ApplicationConfiguration> applicationConfiguration,
            IClusterPowerStatusRepository clusterPowerStatusRepository,
            IRaspbeeService raspbeeService,
            ILogger<CheckHostStatusHandler> logger)
        {
            _shutdownInitiatedEventSender = shutdownInitiatedShutdownInitiatedEventSender;
            _shutdownCompletedEventSender = shutdownCompletedEventSender;
            _clusterConfiguration = clusterConfiguration.Value;
            _clusterPowerStatusRepository = clusterPowerStatusRepository;
            _raspbeeService = raspbeeService;
            _logger = logger;
        }

        public async Task<bool> Handle(CheckHostStatus request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Checking '{Hostname}' since '{CheckingSince} s'",
                request?.Hostname,
                request?.InitiatedTime != null
                    ? DateTime.Now.Subtract(request.InitiatedTime).TotalSeconds
                    : Double.NaN);

            var requestHostname = request.Hostname;
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
                    // e.g. hostname does not resolve (container), assume its down
                    _logger.LogDebug("Ping failed: {PingError}", e.Error.ToString());

                    await ConcludePowerOff(requestHostname);

                    // Let the main thread resume.
                    ((AutoResetEvent) e.UserState).Set();

                    return;
                }

                var reply = e.Reply;

                if (reply != null && reply.Status == IPStatus.TimedOut)
                {
                    await ConcludePowerOff(requestHostname);
                }

                LogReply(reply);

                if (reply != null && reply.Status == IPStatus.Success)
                {
                    Thread.Sleep(1000);

                    await _shutdownInitiatedEventSender.Send(new ShutdownInitiated
                    {
                        Hostname = requestHostname,
                        InitiatedTime = request.InitiatedTime
                    });
                }

                // Let the main thread resume.  
                ((AutoResetEvent) e.UserState).Set();
            };

            var data = "xxx";
            var buffer = Encoding.ASCII.GetBytes(data);

            // Wait 1 second for a reply.  
            const int timeout = 1000;

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
            pingSender.SendAsync(requestHostname, timeout, buffer, options, waiter);

            // Prevent this example application from ending. 
            // A real application should do something useful  
            // when possible.  
            waiter.WaitOne();

            // _logger.LogDebug("WaitOne returned, {Hostname}", request?.Hostname);
            return await Task.FromResult(true);
        }

        private async Task ConcludePowerOff(string requestHostname)
        {
            // todo: this needs actually be converted to a SAGA...
            //       n individual hosts are shut down, when all are off, switch off power
            
            // set host status to off
            await _clusterPowerStatusRepository.EnsureHostPowerStatus(_clusterConfiguration.ClusterId,
                requestHostname, new HostPowerStatus
                {
                    Hostname = requestHostname,
                    Status = PowerStatus.Off
                });

            var clusterPowerStatus =
                await _clusterPowerStatusRepository.GetPowerStatus(_clusterConfiguration.ClusterId);

            var sb = clusterPowerStatus.HostsPower.Aggregate(new StringBuilder(), (builder, pair) =>
            {
                builder.Append((string?) $"{pair.Key}: {pair.Value.Status} | ");
                return builder;
            });

            if (sb != null) _logger.LogDebug(sb.ToString());

            // check if we can turn off (all hosts off)
            if (clusterPowerStatus
                .HostsPower
                .All(kvp =>
                    kvp.Value.Status == PowerStatus.Off))
            {
                _raspbeeService.PowerOff();
                await _shutdownCompletedEventSender.Send(new ShutdownCompleted());
            }
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