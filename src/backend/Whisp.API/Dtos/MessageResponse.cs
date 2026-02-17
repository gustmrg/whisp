namespace Whisp.API.Dtos;

public record MessageResponse(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string SenderDisplayName,
    string Body,
    DateTime CreatedAt,
    DateTime? SentAt);
