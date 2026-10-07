using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ModelessModule.Messages;

/// <summary>
///     Represents a request to activate the registered window.
/// </summary>
public sealed class FocusRequestMessage : RequestMessage<bool>;
