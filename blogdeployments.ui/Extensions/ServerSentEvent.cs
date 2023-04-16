namespace blogdeployments.ui.Extensions;

public class ServerSentEvent
{
    public string Name { get; set; }
    public object Data { get; set; }
    public string Id { get; set; }
    public int? Retry { get; set; }

    public ServerSentEvent(string name, object data)
    {
        Name = name;
        Data = data;
    }
}