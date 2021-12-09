using System.Net;
using blogdeployments.power.Service;
using MediatR;

namespace blogdeployments.power.Handler;

public class CheckHostStatus : IRequest<bool>
{
    public IPAddress[] Adresses { get; set; }

    // fixme: return bool not necessary
    public class CheckHostStatusHandler : IRequestHandler<CheckHostStatus, bool>
    {
        private readonly IRaspbeeService _raspbeeService;

        public CheckHostStatusHandler(IRaspbeeService raspbeeService)
        {
            _raspbeeService = raspbeeService;
        }
        public Task<bool> Handle(CheckHostStatus request, CancellationToken cancellationToken)
        {
            // todo: Wait for ping to fail

            return Task.FromResult(true);
        }
    }
}



