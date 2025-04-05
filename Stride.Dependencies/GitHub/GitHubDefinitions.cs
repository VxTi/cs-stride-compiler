using System.Text.Json.Serialization;

namespace Stride.Dependencies.GitHub;

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

public struct ReleaseTagInfo
{
    [JsonPropertyName("object")] public ReleaseTagObject Object { get; set; }
}

[Serializable]
public struct Release
{
    [JsonPropertyName("zipball_url")] public string ZipUrl { get; set; }

    [JsonPropertyName("prerelease")] public bool PreRelease { get; set; }

    [JsonPropertyName("tag_name")] public string TagName { get; set; }

    [JsonPropertyName("assets")] public Asset[] Assets { get; set; }
}