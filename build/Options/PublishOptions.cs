namespace Build.Options;

[Serializable]
public sealed record PublishOptions
{
    public string? Version { get; init; }
}
