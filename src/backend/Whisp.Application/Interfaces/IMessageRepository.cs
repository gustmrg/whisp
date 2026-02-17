using Whisp.Domain.Entities;

namespace Whisp.Application.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<IReadOnlyList<Message>> GetByConversationIdAsync(Guid conversationId, int skip, int take, CancellationToken cancellationToken = default);
    Task<Message?> GetLatestByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid conversationId, DateTime lastReadAt, CancellationToken cancellationToken = default);
}
