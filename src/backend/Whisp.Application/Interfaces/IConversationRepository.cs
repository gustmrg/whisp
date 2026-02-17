using Whisp.Domain.Entities;

namespace Whisp.Application.Interfaces;

/// <summary>
/// Repository interface for <see cref="Conversation"/> and <see cref="ConversationMember"/> operations.
/// </summary>
public interface IConversationRepository : IRepository<Conversation>
{
    /// <summary>
    /// Retrieves a conversation by its identifier, including its members.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The conversation with members loaded if found; otherwise, <c>null</c>.</returns>
    Task<Conversation?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a conversation by its identifier, including its messages.
    /// </summary>
    /// <param name="id">The conversation identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The conversation with messages loaded if found; otherwise, <c>null</c>.</returns>
    Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all conversations that a user is a member of.
    /// </summary>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of conversations the user belongs to.</returns>
    Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the direct conversation between two users, if one exists.
    /// </summary>
    /// <param name="userId1">The first user's identifier.</param>
    /// <param name="userId2">The second user's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The direct conversation if found; otherwise, <c>null</c>.</returns>
    Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a member to a conversation.
    /// </summary>
    /// <param name="member">The conversation member to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddMemberAsync(ConversationMember member, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a member from a conversation.
    /// </summary>
    /// <param name="member">The conversation member to remove.</param>
    void RemoveMember(ConversationMember member);

    /// <summary>
    /// Retrieves a specific member of a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The conversation member if found; otherwise, <c>null</c>.</returns>
    Task<ConversationMember?> GetMemberAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a user is a member of a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if the user is a member; otherwise, <c>false</c>.</returns>
    Task<bool> IsMemberAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all members of a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of conversation members.</returns>
    Task<IReadOnlyList<ConversationMember>> GetMembersAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the last read timestamp for a user in a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="lastReadAt">The timestamp of the last read message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UpdateLastReadAsync(Guid conversationId, Guid userId, DateTime lastReadAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the admin member of a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The admin conversation member if found; otherwise, <c>null</c>.</returns>
    Task<ConversationMember?> GetAdminAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a user is an admin of a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation identifier.</param>
    /// <param name="userId">The user's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if the user is an admin; otherwise, <c>false</c>.</returns>
    Task<bool> IsAdminAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);
}
