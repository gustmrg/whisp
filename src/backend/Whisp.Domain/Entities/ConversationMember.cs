namespace Whisp.Domain.Entities;

public class ConversationMember
{
    public Guid ConversationId { get; set; }
    public required Conversation Conversation { get; set; }
    public Guid UserId { get; set; }
    public required User User { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }
}
