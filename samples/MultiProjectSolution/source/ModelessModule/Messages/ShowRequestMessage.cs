using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ModelessModule.Messages;

/// <summary>
///     The request to show the registered window.
/// </summary>
public sealed class ShowRequestMessage : RequestMessage<bool>;