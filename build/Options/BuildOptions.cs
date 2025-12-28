using System.ComponentModel.DataAnnotations;

namespace Build.Options;

[Serializable]
public sealed record BuildOptions
{
    public string? Version { get; init; }
    [Required] public string OutputDirectory { get; init; } = null!;
}