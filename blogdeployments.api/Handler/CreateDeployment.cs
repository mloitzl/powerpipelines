using AutoMapper;
using blogdeployments.domain;
using blogdeployments.repository;
using MediatR;

namespace blogdeployments.api.Handler;

public class CreateDeployment : IRequest<Deployment>
{
    public string? Hash { get; set; }
    public string? FriendlyName { get; set; }

    public class CreateDeploymentHandler : IRequestHandler<CreateDeployment, Deployment>
    {
        private readonly IMapper _mapper;
        private readonly IDeploymentsRepository _repository;

        public CreateDeploymentHandler(
            IDeploymentsRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Deployment> Handle(CreateDeployment createDeployment, CancellationToken cancellationToken)
        {
            var deployment = _mapper.Map<Deployment>(createDeployment);

            return _mapper.Map<Deployment>(await _repository.CreateDeployment(deployment));
        }
    }
}