using AutoMapper;
using blogdeployments.api.Sender;
using blogdeployments.domain;
using MediatR;

namespace blogdeployments.api.Handler;

public class CompleteDeployment : IRequest<Deployment>
{
    public string? Hash { get; set; }
    public string? FriendlyName { get; set; }
    public class CompleteDeploymentHandler : IRequestHandler<CompleteDeployment, Deployment>
    {
        private readonly ICompleteDeploymentSender _sender;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CompleteDeploymentHandler(
            ICompleteDeploymentSender sender,
            IMediator mediator,
            IMapper mapper)
        {
            _sender = sender;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<Deployment> Handle(CompleteDeployment request, CancellationToken cancellationToken)
        {
            var deployment = _mapper.Map<Deployment>(request);

            await _sender.Send(deployment);

            return deployment;
        }
    }

}
