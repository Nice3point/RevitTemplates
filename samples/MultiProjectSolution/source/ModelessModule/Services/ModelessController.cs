using System.Windows;
using Visibility = System.Windows.Visibility;

namespace ModelessModule.Services;

public sealed class ModelessController
{
    private Window _window;

    /// <summary>
    ///     Attempts to set focus to this element
    /// </summary>
    /// <returns>True if the window instance has already been created</returns>
    public bool Focus()
    {
        if (_window.WindowState == WindowState.Minimized) _window.WindowState = WindowState.Normal;
        if (_window.Visibility != Visibility.Visible) _window.Show();
        return _window.Focus();
    }

    /// <summary>Opens a window and returns without waiting for the newly opened _window to close</summary>
    public void Show(Window window)
    {
        RegisterWindow(window);
        window.Show(Context.UiApplication.MainWindowHandle);
    }

    /// <summary>
    ///     Makes an active window visible
    /// </summary>
    public void Show()
    {
        _window?.Show();
    }

    /// <summary>
    ///     Makes an active window invisible
    /// </summary>
    public void Hide()
    {
        _window?.Hide();
    }

    /// <summary>
    ///     Manually closes an active window />
    /// </summary>
    public void Close()
    {
        _window?.Close();
    }

    private void RegisterWindow(Window window)
    {
        _window = window;
        _window.Closed += (_, _) =>
        {
            _window = null;
        };
    }
}