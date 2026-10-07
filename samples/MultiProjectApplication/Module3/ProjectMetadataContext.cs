using Autodesk.Revit.DB.ExtensibleStorage;

namespace Module3;

/// <summary>
///     Provides access to the <see cref="ProjectMetadata" /> of a document.
/// </summary>
/// <param name="document">The document that stores the project metadata.</param>
public sealed class ProjectMetadataContext(Document document)
{
    private const string StorageName = "RevitAddIn DataStorage";

    private readonly Schema _schema = ProjectMetadataConfiguration.Create();

    /// <summary>
    ///     Reads the project metadata from the document.
    /// </summary>
    /// <returns>The stored project metadata, or empty metadata if the document has no metadata.</returns>
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
    ///     Writes the project metadata to the document.
    /// </summary>
    /// <param name="data">The project metadata to write.</param>
    /// <remarks>The caller starts the transaction. If the document has no storage element, the method creates it.</remarks>
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
