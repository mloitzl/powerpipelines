using AutoMapper;
using blogdeployments.domain;
using CouchDB.Driver.Extensions;

namespace blogdeployments.repository;

public class DeploymentsRepository : IDeploymentsRepository
{
    private readonly DeploymentsContext _context;
    private readonly IMapper _mapper;

    public DeploymentsRepository(
        DeploymentsContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Deployment> CreateDeployment(Deployment deployment)
    {
        var deploymentDoc =
            await _context
                .Deployments
                .AddAsync(
                    _mapper.Map<DeploymentDocument>(deployment)
                );

        return _mapper.Map<Deployment>(deploymentDoc);
    }
    public async Task<IEnumerable<Deployment>> GetDeployments()
    {
        // var lineCount = 0;
        var tokenSource = new CancellationTokenSource();
        // var x = await _context.Deployments.FindManyAsync(new []{ "55f3df55aa9067ae77ba8cb81b00076e" }, tokenSource.Token);
        // var y = await _context.Deployments.Where(document => document.FriendlyName == "Hello World")
        //     .ToListAsync(tokenSource.Token);
        try
        {
            var x =
                await
                    _context
                        .ClusterPower
                        .Where(d => d.Id != null)
                        .Take(1)
                        .ToListAsync(tokenSource.Token);

            return x.Select(document => _mapper.Map<Deployment>(document));
        }
        catch (Exception ex)
        {
            throw;
        }
        // await foreach (var deploymentChange in _context
        //                    .Deployments
        //                    .GetContinuousChangesAsync(
        //                        null, 
        //                        null, 
        //                        tokenSource.Token))
        // {
        //     lineCount++;
        //     if (lineCount == 20) tokenSource.Cancel();
        //     result.Add(_mapper.Map<Deployment>(deploymentChange.Document));
        // }
    }

    public Task<IEnumerable<Deployment>> GetDeployment(string id)
    {
        throw new NotImplementedException();
    }
}