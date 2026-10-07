using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RevitAddIn.Messages;

/// <summary>
///     Represents a request to show the registered window.
/// </summary>
public sealed class ShowRequestMessage : RequestMessage<bool>;
