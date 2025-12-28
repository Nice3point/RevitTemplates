using System.ComponentModel.DataAnnotations;

namespace Build.Options;

[Serializable]
public sealed record BuildOptions
{
    [Required] public string Version { get; init; } = "1.0.0";
    [Required] public string OutputDirectory { get; init; } = null!;
}