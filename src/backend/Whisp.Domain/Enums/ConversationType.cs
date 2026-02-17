namespace Whisp.Domain.Enums;

/// <summary>
/// Represents the type of a conversation.
/// </summary>
public enum ConversationType
{
    /// <summary>
    /// A one-on-one conversation between two users.
    /// </summary>
    Direct = 0,

    /// <summary>
    /// A conversation with multiple participants.
    /// </summary>
    Group = 1
}