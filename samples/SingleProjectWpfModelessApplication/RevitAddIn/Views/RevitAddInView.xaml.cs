using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using RevitAddIn.Messages;
using RevitAddIn.ViewModels;

namespace RevitAddIn.Views;

public sealed partial class RevitAddInView
{
    public RevitAddInView(RevitAddInViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
    
    private static void OnLoaded(object sender, RoutedEventArgs args)
    {
        var self = (RevitAddInView)sender;
        StrongReferenceMessenger.Default.Register<RevitAddInView, FocusRequestMessage>(self, (recipient, message) =>
        {
            recipient.Activate();
            message.Reply(recipient.Focus());
        });
    }

    private static void OnUnloaded(object sender, RoutedEventArgs args)
    {
        var self = (RevitAddInView)sender;
        StrongReferenceMessenger.Default.UnregisterAll(self);
    }
}