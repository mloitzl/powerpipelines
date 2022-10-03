using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace blogdeployments.api.Model;

public class GetDeploymentsViewModel
{
    [JsonPropertyName("number")]
    [FromQuery(Name = "number")]
    public int? Number { get; set; }
}