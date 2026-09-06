using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.Options;

namespace RevitAddIn.Commands;

/// <summary>
///     Provides the Revit external command entry point.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var selectionConfiguration = new SelectionConfiguration()
            .Allow.Element(element => element is Wall);

        var reference = Application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
        var element = reference.ElementId.ToElement(Application.ActiveUIDocument.Document)!;

        TaskDialog.Show("Selected element", element.Name);
    }
}
