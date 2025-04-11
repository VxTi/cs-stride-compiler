using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Stride.Common.Packages;

namespace Stride.Common;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(Release))]
[JsonSerializable(typeof(Asset))]
[JsonSerializable(typeof(Package))]
[JsonSerializable(typeof(List<Package>))]
[JsonSerializable(typeof(ProjectConfig))]
[JsonSerializable(typeof(ReleaseTagInfo))]
[JsonSerializable(typeof(ReleaseTagObject))]
internal partial class JsonSerializationContext : JsonSerializerContext
{
    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        IndentSize = 2
    };

    public static readonly JsonSerializationContext Instance = new(SerializeOptions);

    public static JsonTypeInfo GetTypeInfo<T>()
    {
        return typeof(T) switch
        {
            var t when t == typeof(Release) => Default.Release,
            var t when t == typeof(Asset) => Default.Asset,
            var t when t == typeof(Package) => Default.Package,
            var t when t == typeof(ProjectConfig) => Default.ProjectConfig,
            var t when t == typeof(ReleaseTagInfo) => Default.ReleaseTagInfo,
            var t when t == typeof(ReleaseTagObject) => Default.ReleaseTagObject,
            var t when t == typeof(List<Package>) => Default.ListPackage,
            _ => throw new NotSupportedException($"Deserialization for {typeof(T).Name} is not supported.")
        };
    }
}