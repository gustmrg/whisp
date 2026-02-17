namespace Whisp.API.Dtos;

public record ConversationResponse(
    Guid Id,
    string Type,
    DateTime CreatedAt,
    List<MemberResponse> Members,
    MessageResponse? LastMessage);

public record MemberResponse(
    Guid UserId,
    string DisplayName,
    string Role);
