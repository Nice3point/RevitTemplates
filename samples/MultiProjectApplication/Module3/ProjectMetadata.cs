namespace Module3;

/// <summary>
///     Represents the project record the add-in stores in the document.
/// </summary>
public sealed class ProjectMetadata
{
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
