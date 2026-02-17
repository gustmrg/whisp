namespace Whisp.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user attempts to access a conversation they are not a member of.
/// </summary>
/// <param name="userId">The identifier of the unauthorized user.</param>
/// <param name="conversationId">The identifier of the conversation.</param>
public class UnauthorizedMemberException(Guid userId, Guid conversationId)
    : Exception($"User '{userId}' is not a member of conversation '{conversationId}'.")
{
    /// <summary>
    /// Gets the identifier of the unauthorized user.
    /// </summary>
    public Guid UserId { get; } = userId;

    /// <summary>
    /// Gets the identifier of the conversation the user attempted to access.
    /// </summary>
    public Guid ConversationId { get; } = conversationId;
}
