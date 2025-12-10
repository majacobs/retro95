using System.Text.Json.Serialization;

namespace Retro95.Models;

public class GiphyContent
{
    [JsonPropertyName("caption")]
    public required string Caption { get; init; }

    [JsonPropertyName("image")]
    public required string Image { get; init; }
    
    [JsonPropertyName("title")]
    public required string Title { get; init; }
}