using Whisp.Domain.Entities;

namespace Whisp.Application.Interfaces;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken = default);

    // ConversationMember operations
    Task AddMemberAsync(ConversationMember member, CancellationToken cancellationToken = default);
    void RemoveMember(ConversationMember member);
    Task<ConversationMember?> GetMemberAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConversationMember>> GetMembersAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task UpdateLastReadAsync(Guid conversationId, Guid userId, DateTime lastReadAt, CancellationToken cancellationToken = default);
}
