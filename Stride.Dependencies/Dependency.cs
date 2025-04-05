namespace Stride.Dependencies;

public record Dependency(
    string Version,
    string Author,
    string Name,
    string Sha256
);