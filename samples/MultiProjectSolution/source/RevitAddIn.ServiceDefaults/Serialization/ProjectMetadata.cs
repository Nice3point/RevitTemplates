namespace RevitAddIn.ServiceDefaults.Serialization;

/// <summary>
///     Represents the project data that the application logs after the document is saved.
/// </summary>
/// <param name="ProjectName">The project name after the document is saved.</param>
public sealed record ProjectMetadata(string? ProjectName);
