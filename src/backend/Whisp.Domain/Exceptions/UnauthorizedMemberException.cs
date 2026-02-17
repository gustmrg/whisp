namespace Whisp.Domain.Exceptions;

public class UnauthorizedMemberException(Guid userId, Guid conversationId)
    : Exception($"User '{userId}' is not a member of conversation '{conversationId}'.")
{
    public Guid UserId { get; } = userId;
    public Guid ConversationId { get; } = conversationId;
}
