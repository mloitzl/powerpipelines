using System.Net;
using System.Runtime.InteropServices;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using MediatR;
using Microsoft.Extensions.Options;
using static blogdeployments.agent.Native.NativeMethods;

namespace blogdeployments.agent.Handler;

public class Shutdown : IRequest<bool>
{
    public class ShutdownHandler : IRequestHandler<Shutdown, bool>
    {
        private readonly ILogger<ShutdownHandler> _logger;
        private readonly IOptions<AgentConfiguration> _options;
        private readonly IEventSender<ShutdownInitiated> _sender;

        public ShutdownHandler(
            IEventSender<ShutdownInitiated> sender,
            IOptions<AgentConfiguration> options,
            ILogger<ShutdownHandler> logger)

        {
            _sender = sender;
            _options = options;
            _logger = logger;
        }

        public async Task<bool> Handle(Shutdown request, CancellationToken cancellationToken)
        {
            var hostName = Dns.GetHostName();

            _logger.LogDebug("ShutdownHandler.Handle '{HostName}'", hostName);

            var @event = new ShutdownInitiated
            {
                Hostname = hostName,
                InitiatedTime = DateTime.Now
            };

            await _sender.Send(@event);

            if (_options.Value.RunningInContainer)
            {
                _logger.LogDebug("## NOTE: Running in a Container. Killing container by exiting with code 1");
                Environment.Exit(1);
                return true;
            }

            if (_options.Value.DryRun)
            {
                _logger.LogDebug("## NOTE: DryRun. Not shutting down myself");
                return true;
            }


            _logger.LogDebug("Trying to send shutdown native call...");

            sync();
            // Int32 ret = reboot(RB_POWER_OFF); // too hard, doesn't run systemd scripts
            var ret = system("shutdown -h 0");

            // todo: check if these are correct for 'system(...)' syscall
            if (ret == 0) throw new InvalidOperationException("reboot(LINUX_REBOOT_CMD_POWER_OFF) returned 0.");

            if (ret != -1) throw new InvalidOperationException("Unexpected reboot() return value: " + ret);

            var errno = Marshal.GetLastWin32Error();

            switch (errno)
            {
                case EPERM:
                    throw new UnauthorizedAccessException("You do not have permission to call reboot()");

                case EINVAL:
                    throw new ArgumentException("Bad magic numbers (stray cosmic-ray?)");

                case EFAULT:
                default:
                    throw new InvalidOperationException("Could not call reboot():" + errno);
            }
        }
    }
}