using blogdeployments.power.Service;
using MediatR;

namespace blogdeployments.power.Handler;

public class PowerOff : IRequest<bool>
{
    public class PowerOffHandler : IRequestHandler<PowerOff, bool>
    {
        private readonly IRaspbeeService _raspbeeService;

        public PowerOffHandler(IRaspbeeService raspbeeService)
        {
            _raspbeeService = raspbeeService;
        }
        public Task<bool> Handle(PowerOff request, CancellationToken cancellationToken)
        {
            // todo: Wait for ping to fail
            return Task.FromResult(_raspbeeService.PowerOff());
        }
    }
}



