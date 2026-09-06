using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using ModelessModule.Messages;
using ModelessModule.ViewModels;

namespace ModelessModule.Views;

/// <summary>
///     Represents the add-in window.
/// </summary>
public sealed partial class ModelessModuleView :
    IRecipient<ShowRequestMessage>,
    IRecipient<HideRequestMessage>,
    IRecipient<FocusRequestMessage>
{
    private readonly IMessenger _messenger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ModelessModuleView" /> class.
    /// </summary>
    /// <param name="viewModel">The view model associated with the window.</param>
    /// <param name="messenger">The messenger for window visibility requests.</param>
    public ModelessModuleView(ModelessModuleViewModel viewModel, IMessenger messenger)
    {
        _messenger = messenger;
        DataContext = viewModel;
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        _messenger.RegisterAll(this);
    }

    private void OnUnloaded(object sender, RoutedEventArgs args)
    {
        _messenger.UnregisterAll(this);
    }

    public void Receive(ShowRequestMessage message)
    {
        Show();
    }

    public void Receive(HideRequestMessage message)
    {
        Hide();
    }

    public void Receive(FocusRequestMessage message)
    {
        Activate();
        message.Reply(Focus());
    }
}
