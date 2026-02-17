using Whisp.Domain.Enums;

namespace Whisp.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public ConversationType Type { get; set; } = ConversationType.Direct;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public required User Creator { get; set; }
    public ICollection<ConversationMember> Members { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
}
