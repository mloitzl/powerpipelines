namespace blogdeployments.agent;

public class AgentConfiguration
{
    public bool DryRun { get; set; } = false;
    //todo: this is set by .net core automatically in env 'DOTNET_RUNNING_IN_CONTAINER'
    public bool RunningInContainer { get; set; } = false;
}