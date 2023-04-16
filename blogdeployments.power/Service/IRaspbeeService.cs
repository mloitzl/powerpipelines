namespace blogdeployments.power.Service;

public interface IRaspbeeService
{
    bool PowerOn();
    bool PowerOff();
    bool IsOn { get; }
}