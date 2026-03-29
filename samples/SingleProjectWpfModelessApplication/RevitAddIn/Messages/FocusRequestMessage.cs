using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RevitAddIn.Messages;

/// <summary>
///     The request to set the focus for the registered window.
/// </summary>
public sealed class FocusRequestMessage : RequestMessage<bool>;