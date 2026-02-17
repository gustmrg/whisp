namespace Whisp.API.Dtos;

public record SendMessageRequest(Guid ConversationId, string Body);
