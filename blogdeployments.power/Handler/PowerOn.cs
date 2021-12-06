using blogdeployments.power.Service;
using MediatR;

namespace blogdeployments.power.Handler;

public class PowerOn : IRequest<bool>
{
    public class PowerOnHandler : IRequestHandler<PowerOn, bool>
    {
        private readonly IRaspbeeService _raspbeeService;

        public PowerOnHandler(IRaspbeeService raspbeeService)
        {
            _raspbeeService = raspbeeService;
        }
        public Task<bool> Handle(PowerOn request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_raspbeeService.PowerOn());
        }
    }
}



