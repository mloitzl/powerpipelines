namespace blogdeployments.domain;

public interface IEventSender<T>
    where T : IEvent
{
    Task Send(T @event);
}