using Whisp.Domain.Enums;

namespace Whisp.Domain.Entities;

/// <summary>
/// Represents the membership of a user in a conversation.
/// </summary>
public class ConversationMember
{
    /// <summary>
    /// Gets or sets the conversation identifier.
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the associated conversation.
    /// </summary>
    public required Conversation Conversation { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the associated user.
    /// </summary>
    public required User User { get; set; }

    /// <summary>
    /// Gets or sets the date and time the user joined the conversation.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the user last read messages in this conversation.
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    /// <summary>
    /// Gets or sets the role of the user in this conversation.
    /// </summary>
    public ConversationRole Role { get; set; } = ConversationRole.Member;
}
