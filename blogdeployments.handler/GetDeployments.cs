using blogdeployments.domain;
using blogdeployments.repository;
using MediatR;

namespace blogdeployments.handler;

public class GetDeployments : IRequest<IEnumerable<Deployment>>
{
    public class GetDeploymentsHandler : IRequestHandler<GetDeployments, IEnumerable<Deployment>>
    {
        private readonly IDeploymentsRepository _repository;

        public GetDeploymentsHandler(IDeploymentsRepository repository)
        {
            _repository = repository;
        }
        public Task<IEnumerable<Deployment>> Handle(GetDeployments request, CancellationToken cancellationToken)
        {
            return _repository.GetDeployments();
        }
    }
}