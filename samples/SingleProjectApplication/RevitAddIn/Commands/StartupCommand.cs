using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.Options;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var selectionConfiguration = new SelectionConfiguration()
            .Allow.Element(element => element is Wall);

        var reference = UiDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter);
        var element = reference.ElementId.ToElement(Document)!;

        TaskDialog.Show("Selected element",element.Name);
    }
}