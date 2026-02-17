using Whisp.Domain.ValueObjects;

namespace Whisp.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public required MessageContent Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }

    public required Conversation Conversation { get; set; }
    public required User Sender { get; set; }
}
