using Microsoft.EntityFrameworkCore;
using Whisp.Application.Interfaces;
using Whisp.Domain.Entities;

namespace Whisp.Infrastructure.Persistence.Repositories;

public class MessageRepository(AppDbContext context) : Repository<Message>(context), IMessageRepository
{
    public async Task<IReadOnlyList<Message>> GetByConversationIdAsync(
        Guid conversationId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Include(m => m.Sender)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetLatestByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid conversationId, DateTime lastReadAt, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .CountAsync(m => m.ConversationId == conversationId && m.CreatedAt > lastReadAt, cancellationToken);
    }
}
