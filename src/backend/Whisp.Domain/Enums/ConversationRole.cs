namespace Whisp.Domain.Enums;

/// <summary>
/// Represents the role of a user within a conversation.
/// </summary>
public enum ConversationRole
{
    /// <summary>
    /// A regular conversation member with no special privileges.
    /// </summary>
    Member = 0,

    /// <summary>
    /// An administrator with elevated privileges (e.g. managing members in group conversations).
    /// </summary>
    Admin = 1
}
