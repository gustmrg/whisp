namespace Whisp.Domain.Exceptions;

/// <summary>
/// Exception thrown when a conversation with the specified identifier cannot be found.
/// </summary>
/// <param name="conversationId">The identifier of the conversation that was not found.</param>
public class ConversationNotFoundException(Guid conversationId)
    : Exception($"Conversation with id '{conversationId}' was not found.")
{
    /// <summary>
    /// Gets the identifier of the conversation that was not found.
    /// </summary>
    public Guid ConversationId { get; } = conversationId;
}
