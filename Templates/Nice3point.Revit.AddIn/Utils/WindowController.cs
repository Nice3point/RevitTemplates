using System.Windows;

namespace Nice3point.Revit.AddIn.Utils;

public static class WindowController
{
    private static readonly List<Window> Windows = new();

    /// <summary>
    ///     Attempts to set focus to this element
    /// </summary>
    /// <typeparam name="T">Type of window</typeparam>
    /// <returns>True if the window instance has already been created</returns>
    public static bool Focus<T>() where T : Window
    {
        var type = typeof(T);
        foreach (var window in Windows)
            if (window.GetType() == type)
            {
                if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;
                window.Focus();
                return true;
            }

        return false;
    }

    /// <summary>Opens a window and returns without waiting for the newly opened window to close</summary>
    public static void Show(Window window)
    {
        Attach(window);
        window.Show();
    }

    /// <summary>
    ///     Opens a window and returns without waiting for the newly opened window to close. Sets the owner of a child window
    /// </summary>
    public static void Show(Window window, IntPtr handle)
    {
        Attach(window);
        window.Show(handle);
    }

    private static void Attach(Window window)
    {
        Windows.Add(window);
        window.Closed += (sender, _) =>
        {
            var modelessWindow = (Window) sender;
            Windows.Remove(modelessWindow);
        };
    }
}