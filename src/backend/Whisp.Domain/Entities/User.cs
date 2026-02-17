namespace Whisp.Domain.Entities;

/// <summary>
/// Represents a user of the messaging platform.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique username.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the date and time the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the conversation memberships for this user.
    /// </summary>
    public ICollection<ConversationMember> ConversationMembers { get; set; } = [];

    /// <summary>
    /// Gets or sets the messages sent by this user.
    /// </summary>
    public ICollection<Message> SentMessages { get; set; } = [];
}
