namespace Whisp.API.Dtos;

public record SendDirectMessageRequest(Guid ParticipantId, string Body);
