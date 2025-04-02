﻿using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using RevitAddIn.ViewModels;
using RevitAddIn.Views;

namespace RevitAddIn.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var view = Host.GetService<RevitAddInView>();
        view.ShowDialog();
    }
}