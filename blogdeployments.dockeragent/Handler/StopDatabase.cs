using MediatR;

public class StopDatabase : IRequest<bool>
{
    public class StopDatabaseHandler: IRequestHandler<StopDatabase, bool>
    {
        public Task<bool> Handle(StopDatabase request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Issue Stop Container Commandline");
            return Task.FromResult(true);
        }
    }
}