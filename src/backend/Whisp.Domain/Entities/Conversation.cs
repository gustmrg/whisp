using Whisp.Domain.Enums;

namespace Whisp.Domain.Entities;

/// <summary>
/// Represents a conversation between users.
/// </summary>
public class Conversation
{
    /// <summary>
    /// Gets or sets the unique identifier for the conversation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of conversation (direct or group).
    /// </summary>
    public ConversationType Type { get; set; } = ConversationType.Direct;

    /// <summary>
    /// Gets or sets the date and time the conversation was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the members of the conversation.
    /// </summary>
    public ICollection<ConversationMember> Members { get; set; } = [];

    /// <summary>
    /// Gets or sets the messages in the conversation.
    /// </summary>
    public ICollection<Message> Messages { get; set; } = [];
}
