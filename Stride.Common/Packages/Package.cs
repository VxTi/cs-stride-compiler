using System.Text.Json.Serialization;

namespace Stride.Common.Packages;

[Serializable]
public class Package
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("version")] public required string Version { get; set; }

    [JsonPropertyName("author")] public required string Author { get; set; }

    [JsonPropertyName("sha256")] public required string Sha256 { get; set; }
}