using Microsoft.EntityFrameworkCore;
using Whisp.Application.Interfaces;
using Whisp.Domain.Entities;
using Whisp.Domain.Enums;

namespace Whisp.Infrastructure.Persistence.Repositories;

public class ConversationRepository(AppDbContext context) : Repository<Conversation>(context), IConversationRepository
{
    public async Task<Conversation?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.Members.Any(m => m.UserId == userId))
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.Type == ConversationType.Direct)
            .Where(c => c.Members.Any(m => m.UserId == userId1))
            .Where(c => c.Members.Any(m => m.UserId == userId2))
            .FirstOrDefaultAsync(cancellationToken);
    }

    // ConversationMember operations

    public async Task AddMemberAsync(ConversationMember member, CancellationToken cancellationToken = default)
    {
        await Context.ConversationMembers.AddAsync(member, cancellationToken);
    }

    public void RemoveMember(ConversationMember member)
    {
        Context.ConversationMembers.Remove(member);
    }

    public async Task<ConversationMember?> GetMemberAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.ConversationMembers
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.ConversationId == conversationId && m.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsMemberAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.ConversationMembers
            .AnyAsync(m => m.ConversationId == conversationId && m.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<ConversationMember>> GetMembersAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await Context.ConversationMembers
            .Where(m => m.ConversationId == conversationId)
            .Include(m => m.User)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateLastReadAsync(Guid conversationId, Guid userId, DateTime lastReadAt, CancellationToken cancellationToken = default)
    {
        var member = await Context.ConversationMembers
            .FirstOrDefaultAsync(m => m.ConversationId == conversationId && m.UserId == userId, cancellationToken);

        if (member is not null)
        {
            member.LastReadAt = lastReadAt;
        }
    }
}
