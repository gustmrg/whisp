namespace Whisp.Domain.ValueObjects;

/// <summary>
/// Value object representing the content of a message, including text, media, and location data.
/// </summary>
public class MessageContent : ValueObject
{
    /// <summary>
    /// Gets the text body of the message.
    /// </summary>
    public string Body { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the URL of the attached media, if any.
    /// </summary>
    public string? MediaUrl { get; private set; }

    /// <summary>
    /// Gets the URL of the media thumbnail, if any.
    /// </summary>
    public string? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Gets the name of the attached file, if any.
    /// </summary>
    public string? FileName { get; private set; }

    /// <summary>
    /// Gets the size of the attached file in bytes, if any.
    /// </summary>
    public long? FileSizeBytes { get; private set; }

    /// <summary>
    /// Gets the duration of the attached media in seconds, if any.
    /// </summary>
    public int? MediaDurationSeconds { get; private set; }

    /// <summary>
    /// Gets the latitude of the shared location, if any.
    /// </summary>
    public double? Latitude { get; private set; }

    /// <summary>
    /// Gets the longitude of the shared location, if any.
    /// </summary>
    public double? Longitude { get; private set; }

    private MessageContent() { }

    /// <summary>
    /// Creates a text-only message content.
    /// </summary>
    /// <param name="body">The text body of the message. Must be non-empty and at most 4096 characters.</param>
    /// <returns>A new <see cref="MessageContent"/> instance with the specified text body.</returns>
    /// <exception cref="ArgumentException">Thrown when the body is empty or exceeds 4096 characters.</exception>
    public static MessageContent CreateText(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Message body cannot be empty.");

        if (body.Length > 4096)
            throw new ArgumentException("Message body cannot exceed 4096 characters.");

        return new MessageContent { Body = body };
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Body;
        yield return MediaUrl;
        yield return ThumbnailUrl;
        yield return FileName;
        yield return FileSizeBytes;
        yield return MediaDurationSeconds;
        yield return Latitude;
        yield return Longitude;
    }
}