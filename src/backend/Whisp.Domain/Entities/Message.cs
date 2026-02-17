using Whisp.Domain.ValueObjects;

namespace Whisp.Domain.Entities;

/// <summary>
/// Represents a message sent within a conversation.
/// </summary>
public class Message
{
    /// <summary>
    /// Gets or sets the unique identifier for the message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the conversation this message belongs to.
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who sent the message.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    public required MessageContent Content { get; set; }

    /// <summary>
    /// Gets or sets the date and time the message was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the message was sent. Null if not yet sent.
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Gets or sets the conversation this message belongs to.
    /// </summary>
    public required Conversation Conversation { get; set; }

    /// <summary>
    /// Gets or sets the user who sent the message.
    /// </summary>
    public required User Sender { get; set; }
}
