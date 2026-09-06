using Autodesk.Revit.DB.ExtensibleStorage;

namespace Module3;

/// <summary>
///     Defines the Extensible Storage schema <see cref="ProjectMetadata" /> is stored in.
/// </summary>
public static class ProjectMetadataConfiguration
{
    public const string Number = "ProjectNumber";
    public const string Name = "ProjectName";
    public const string Address = "ProjectAddress";

    public static readonly Guid Identity = new("0E73AF93-E7F3-42E6-9BA5-AFC1CA23D42B");

    /// <summary>
    ///     Returns the schema registered in the session, and registers it on the first call.
    /// </summary>
    /// <returns>The schema the project record is stored in.</returns>
    public static Schema Create()
    {
        var schema = Schema.Lookup(Identity);
        if (schema is not null) return schema;

        var builder = new SchemaBuilder(Identity)
            .SetSchemaName("RevitAddInDatabase")
            .SetDocumentation("RevitAddIn data storage")
            .SetVendorId("RevitAddIn")
            .SetReadAccessLevel(AccessLevel.Public)
            .SetWriteAccessLevel(AccessLevel.Public);

        builder.AddSimpleField(Number, typeof(string));
        builder.AddSimpleField(Name, typeof(string));
        builder.AddSimpleField(Address, typeof(string));

        return builder.Finish();
    }
}
