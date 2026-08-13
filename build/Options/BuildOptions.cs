namespace Build.Options;

[Serializable]
public sealed record BuildOptions
{
    public string OutputDirectory { get; init; } = "output";
}
