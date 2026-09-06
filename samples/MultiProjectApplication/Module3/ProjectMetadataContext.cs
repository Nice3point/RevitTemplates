using Autodesk.Revit.DB.ExtensibleStorage;

namespace Module3;

/// <summary>
///     Reads and writes the <see cref="ProjectMetadata" /> record of a single document.
/// </summary>
/// <param name="document">The document the record is stored in.</param>
public sealed class ProjectMetadataContext(Document document)
{
    private const string StorageName = "RevitAddIn DataStorage";

    private readonly Schema _schema = ProjectMetadataConfiguration.Create();

    /// <summary>
    ///     Reads the record of the document.
    /// </summary>
    /// <returns>The stored record, or an empty one when the document carries none.</returns>
    public ProjectMetadata Load()
    {
        var storage = FindStorage();
        if (storage is null) return new ProjectMetadata();

        return new ProjectMetadata
        {
            Number = storage.LoadEntity<string>(_schema, ProjectMetadataConfiguration.Number) ?? string.Empty,
            Name = storage.LoadEntity<string>(_schema, ProjectMetadataConfiguration.Name) ?? string.Empty,
            Address = storage.LoadEntity<string>(_schema, ProjectMetadataConfiguration.Address) ?? string.Empty
        };
    }

    /// <summary>
    ///     Writes the record to the document, creating the storage element on the first write.
    /// </summary>
    /// <param name="data">The record to write.</param>
    /// <remarks>The caller opens the transaction.</remarks>
    public void Save(ProjectMetadata data)
    {
        var storage = FindStorage();
        if (storage is null)
        {
            storage = DataStorage.Create(document);
            storage.Name = StorageName;
        }

        storage.SaveEntity(_schema, data.Number, ProjectMetadataConfiguration.Number);
        storage.SaveEntity(_schema, data.Name, ProjectMetadataConfiguration.Name);
        storage.SaveEntity(_schema, data.Address, ProjectMetadataConfiguration.Address);
    }

    private DataStorage? FindStorage()
    {
        return (DataStorage?) document.CollectElements()
            .OfClass<DataStorage>()
            .WithExtensibleStorage(_schema.GUID)
            .FirstOrDefault();
    }
}
