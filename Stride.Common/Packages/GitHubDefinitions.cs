using System.Text.Json.Serialization;

namespace Stride.Common.Packages;

[Serializable]
public struct Asset
{
    [JsonPropertyName("browser_download_url")]
    public string AssetUrl;

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("size")] public int AssetSize { get; set; }

    [JsonPropertyName("content_type")] public string ContentType { get; set; }
}

[Serializable]
public struct ReleaseTagObject
{
    [JsonPropertyName("sha")] public string Sha256 { get; set; }
}

public class ReleaseTagInfo
{
    [JsonRequired]
    [JsonPropertyName("object")]
    public required ReleaseTagObject Object { get; set; }
}

[Serializable]
public class Release
{
    [JsonRequired]
    [JsonPropertyName("zipball_url")]
    public required string ZipUrl { get; set; }

    [JsonRequired]
    [JsonPropertyName("prerelease")]
    public required bool PreRelease { get; set; }

    [JsonRequired]
    [JsonPropertyName("tag_name")]
    public required string TagName { get; set; }

    [JsonRequired]
    [JsonPropertyName("assets")]
    public required Asset[] Assets { get; set; }
}