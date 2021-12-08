using AutoMapper;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using MediatR;

namespace blogdeployments.api.Handler;

public class RegisterDeployment : IRequest<Deployment>
{
    public string? Hash { get; set; }
    public string? FriendlyName { get; set; }

    public class RegisterDeploymentHandler : IRequestHandler<RegisterDeployment, Deployment>
    {
        private readonly IEventSender<PowerOnRequested> _sender;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RegisterDeploymentHandler(
            IEventSender<PowerOnRequested> sender,
            IMediator mediator,
            IMapper mapper)
        {
            _sender = sender;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<Deployment> Handle(RegisterDeployment registerDeployment, CancellationToken cancellationToken)
        {
            var deployment = _mapper.Map<Deployment>(registerDeployment);

            await _sender.Send(new PowerOnRequested());

            return await _mediator.Send(_mapper.Map<CreateDeployment>(deployment));
        }
    }
}
