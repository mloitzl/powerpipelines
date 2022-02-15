using System.Text.Json.Serialization;

namespace blogdeployments.azurereceiver.Model;

public class PipelineViewModel
{
    [JsonPropertyName("action")]
    public ActionType Action { get; set; }
    [JsonPropertyName("sourcebranch")]
    public string SourceBranch { get; set; }
    [JsonPropertyName("commitid")]
    public string CommitId { get; set; }
    [JsonPropertyName("sourceversionmessage")]
    public string SourceVersionMessage { get; set; }
}

public enum ActionType
{
    Unknown,
    Start,
    Complete,
}