using System.Text.Json.Serialization;

namespace Dependencies.GitHub;

public struct Asset
{
    [JsonPropertyName("browser_download_url")]
    public string AssetUrl;
    
    [JsonPropertyName("name")]
    public string Name;
    
    [JsonPropertyName("size")]
    public int AssetSize;
    
    [JsonPropertyName("content_type")]
    public string ContentType;
}

public struct ReleaseTagObject
{
    [JsonPropertyName("sha")]
    public string Sha256;
}

public struct ReleaseTagInfo
{
    [JsonPropertyName("object")]
    public ReleaseTagObject Object;
}
    
public struct Release
{
    [JsonPropertyName("zipball_url")]
    public string ZipUrl;
    
    [JsonPropertyName("prerelease")]
    public bool PreRelease;

    [JsonPropertyName("tag_name")]
    public string TagName;

    [JsonPropertyName("assets")]
    public Asset[] Assets;
}