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
        private readonly IEventSender<ShutdownInitiated> _sender;
        private readonly IOptions<AgentConfigration> _options;
        private readonly ILogger<ShutdownHandler> _logger;

        public ShutdownHandler(
            IEventSender<ShutdownInitiated> sender, 
            IOptions<AgentConfigration> options,
            ILogger<ShutdownHandler> logger)
        
        {
            _sender = sender;
            _options = options;
            _logger = logger;
        }

        public Task<bool> Handle(Shutdown request, CancellationToken cancellationToken)
        {
            _sender.Send(new ShutdownInitiated());

            if (_options.Value.DryRun) return Task.FromResult(true);
            
            _logger.LogDebug("Trying to send shutdown native call...");
            
            sync();
            // Int32 ret = reboot(RB_POWER_OFF); // too hard, doesn't run systemd scripts
            Int32 ret = system("shutdown -h 0");

            // todo: check if these are correct for 'system(...)' syscall
            if (ret == 0) throw new InvalidOperationException("reboot(LINUX_REBOOT_CMD_POWER_OFF) returned 0.");

            if (ret != -1) throw new InvalidOperationException("Unexpected reboot() return value: " + ret);

            Int32 errno = Marshal.GetLastWin32Error();

            switch (errno)
            {
                case EPERM:
                    throw new UnauthorizedAccessException("You do not have permission to call reboot()");

                case EINVAL:
                    throw new ArgumentException("Bad magic numbers (stray cosmic-ray?)");

                case EFAULT:
                default:
                    throw new InvalidOperationException("Could not call reboot():" + errno.ToString());
            }
        }
    }
}