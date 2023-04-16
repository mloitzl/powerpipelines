using Newtonsoft.Json;

namespace blogdeployments.power.Model;

public class SwitchStatus
{
    public SwitchState SwitchState { get; set; }
}

public class SwitchState
{
    public bool Reachable { get; set; }
    public bool On { get; set; }
    [JsonProperty("bri")]
    public decimal Brightness { get; set; }
}