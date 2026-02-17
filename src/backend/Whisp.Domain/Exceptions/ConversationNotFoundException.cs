namespace Whisp.Domain.Exceptions;

public class ConversationNotFoundException(Guid conversationId)
    : Exception($"Conversation with id '{conversationId}' was not found.")
{
    public Guid ConversationId { get; } = conversationId;
}
