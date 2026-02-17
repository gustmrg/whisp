namespace Whisp.API.Dtos;

public record SendDirectMessageResponse(
    Guid ConversationId,
    MessageResponse Message,
    bool ConversationCreated);
