using Autodesk.Revit.DB.ExtensibleStorage;

namespace Module3.Schemas;

public static class ProjectInfoSchema
{
    public static Schema Create()
    {
        var guid = new Guid("0E73AF93-E7F3-42E6-9BA5-AFC1CA23D42B");
        var schema = Schema.Lookup(guid);
        if (schema is not null) return schema;

        var builder = new SchemaBuilder(guid)
            .SetSchemaName("RevitAddInDatabase")
            .SetDocumentation("RevitAddIn data storage")
            .SetVendorId("RevitAddIn")
            .SetReadAccessLevel(AccessLevel.Public)
            .SetWriteAccessLevel(AccessLevel.Public);

        builder.AddSimpleField("ProjectNumber", typeof(string));
        builder.AddSimpleField("ProjectName", typeof(string));
        builder.AddSimpleField("ProjectAddress", typeof(string));

        return builder.Finish();
    }
}