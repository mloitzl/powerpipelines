using MediatR;

namespace blogdeployments.api.Handler;
public class Ping : IRequest<string>
{
    public class PingHandler : IRequestHandler<Ping, string>
    {
        public Task<string> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>("Pong");
        }
    }
}
