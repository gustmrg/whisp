using Whisp.Domain.Entities;

namespace Whisp.Application.Interfaces;

/// <summary>
/// Repository interface for <see cref="Message"/> entity operations.
/// </summary>
public interface IMessageRepository : IRepository<Message>
{
    /// <summary>
    /// Retrieves a paginated list of messages for a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="skip">The number of messages to skip.</param>
    /// <param name="take">The number of messages to return.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of messages.</returns>
    Task<IReadOnlyList<Message>> GetByConversationIdAsync(Guid conversationId, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent message in a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The latest message if the conversation has messages; otherwise, <c>null</c>.</returns>
    Task<Message?> GetLatestByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of unread messages in a conversation since the specified timestamp.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="lastReadAt">The timestamp of the last read message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of unread messages.</returns>
    Task<int> GetUnreadCountAsync(Guid conversationId, DateTime lastReadAt, CancellationToken cancellationToken = default);
}
