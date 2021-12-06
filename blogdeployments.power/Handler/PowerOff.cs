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
            return Task.FromResult(_raspbeeService.PowerOff());
        }
    }
}



