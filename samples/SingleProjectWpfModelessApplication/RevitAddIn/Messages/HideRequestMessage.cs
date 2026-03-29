using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RevitAddIn.Messages;

/// <summary>
///     The request to hide the registered window.
/// </summary>
public sealed class HideRequestMessage : RequestMessage<bool>;