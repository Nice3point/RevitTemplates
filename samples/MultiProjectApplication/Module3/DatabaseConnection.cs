using Autodesk.Revit.DB.ExtensibleStorage;
using Module3.Enums;
using Module3.Schemas;

namespace Module3;

/// <summary>
///     Initializes a new instance of the DatabaseConnection class with the specified entry key.
/// </summary>
public sealed class DatabaseConnection
{
    private readonly Element _storage;
    private readonly Schema _schema;
    
    private Transaction? _transaction;

    /// <summary>
    ///     Initializes a new instance of the DatabaseConnection class with the specified entry key.
    /// </summary>
    /// <param name="entryKey">The entry key used to determine the schema.</param>
    public DatabaseConnection(EntryKey entryKey)
    {
        _schema = entryKey switch
        {
            EntryKey.Data => ProjectInfoSchema.Create(),
            EntryKey.Secrets => ProjectInfoSchema.Create(),
            _ => throw new ArgumentOutOfRangeException(nameof(entryKey), entryKey, null)
        };

        _storage = CreateStorage();
    }

    /// <summary>
    ///     Begins a new transaction for database operations.
    /// </summary>
    public void BeginTransaction()
    {
        _transaction = new Transaction(_storage.Document, "Save data");
        _transaction.Start();
    }

    /// <summary>
    ///     Closes the connection, committing changes.
    /// </summary>
    public void Close()
    {
        if (_transaction is null) return;
        if (!_transaction.IsValidObject) throw new ObjectDisposedException(nameof(DatabaseConnection), "Attempting to close an already closed connection");

        _transaction.Commit();
        _transaction.Dispose();
    }

    /// <summary>
    ///     Saves the specified value associated with a field in the database.
    /// </summary>
    /// <typeparam name="T">The type of the value to be saved.</typeparam>
    /// <param name="field">The field name in which to save the value.</param>
    /// <param name="value">The value to be saved.</param>
    public void Save<T>(string field, T? value) where T : notnull
    {
        if (value == null) return;
        _storage.SaveEntity(_schema, value, field);
    }

    /// <summary>
    /// Loads the value associated with the specified field from the database.
    /// </summary>
    /// <typeparam name="T">The type of the value to be loaded.</typeparam>
    /// <param name="field">The field name from which to load the value.</param>
    /// <returns>The value loaded from the database.</returns>
    public T? Load<T>(string field) where T : notnull
    {
        return _storage.LoadEntity<T>(_schema, field);
    }

    private static DataStorage CreateStorage()
    {
        const string storageName = "RevitAddIn DataStorage";

        var activeDocument = RevitContext.ActiveDocument;
        var storage = (DataStorage?) activeDocument!.CollectElements()
            .Instances()
            .OfClass<DataStorage>()
            .FirstOrDefault(storage => storage.Name == storageName);

        if (storage is not null) return storage;

        using var transaction = new Transaction(activeDocument, "Create data storage");
        transaction.Start();

        storage = DataStorage.Create(activeDocument);
        storage.Name = storageName;

        transaction.Commit();
        return storage;
    }
}