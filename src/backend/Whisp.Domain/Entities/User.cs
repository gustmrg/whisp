namespace Whisp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ConversationMember> ConversationMembers { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
}
