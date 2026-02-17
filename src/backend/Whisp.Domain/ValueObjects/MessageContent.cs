namespace Whisp.Domain.ValueObjects;

public class MessageContent : ValueObject
{
    public string Body { get; private set; } = string.Empty;
    public string? MediaUrl { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public string? FileName { get; private set; }
    public long? FileSizeBytes { get; private set; }
    public int? MediaDurationSeconds { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    
    private MessageContent() { }
    
    public static MessageContent CreateText(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Message body cannot be empty.");

        if (body.Length > 4096)
            throw new ArgumentException("Message body cannot exceed 4096 characters.");

        return new MessageContent { Body = body };
    }
    
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