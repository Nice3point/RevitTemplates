using System.ComponentModel.DataAnnotations;

namespace Build.Options;

[Serializable]
public sealed record PackOptions
{
    [Required] public string Version { get; init; } = null!;
    [Required] public string OutputDirectory { get; init; } = null!;
}