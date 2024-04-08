using Nice3point.Revit.AddIn.Views;
#if (Modeless)
using Nice3point.Revit.AddIn.Utils;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Nice3point.Revit.AddIn.Commands;

/// <summary>
///     Command entry point invoked from the Revit AddIn Application
/// </summary>
#if (Modeless)
public class ShowWindowComponent(IServiceProvider serviceProvider)
#else
public class ShowWindowComponent(Nice3point.Revit.AddInView view)
#endif
{
    public void Execute()
    {
#if (Modeless)
        if (WindowController.Focus<Nice3point.Revit.AddInView>()) return;

        var view = serviceProvider.GetService<Nice3point.Revit.AddInView>();
        WindowController.Show(view, Context.UiApplication.MainWindowHandle);
#elseif (Modal)
        view.ShowDialog();
#endif
    }
}