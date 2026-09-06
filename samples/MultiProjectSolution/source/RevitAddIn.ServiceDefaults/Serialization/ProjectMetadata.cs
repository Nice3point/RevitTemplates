namespace RevitAddIn.ServiceDefaults.Serialization;

/// <summary>
///     Represents the project state the application writes to the log after a save.
/// </summary>
/// <param name="ProjectName">The name the project carries after the save.</param>
public sealed record ProjectMetadata(string? ProjectName);
