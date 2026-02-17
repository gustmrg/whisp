using Microsoft.AspNetCore.SignalR;
using Whisp.API.Dtos;
using Whisp.Application.Interfaces;
using Whisp.Domain.Entities;
using Whisp.Domain.Exceptions;
using Whisp.Domain.ValueObjects;

namespace Whisp.API.Hubs;

public class ChatHub(
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<ChatHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            Context.Abort();
            return;
        }

        var conversations = await conversationRepository.GetByUserIdAsync(userId);
        foreach (var conversation in conversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
        }

        logger.LogInformation("User {UserId} connected with {Count} conversation groups",
            userId, conversations.Count);

        await base.OnConnectedAsync();
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        var userId = GetUserId();

        var isMember = await conversationRepository.IsMemberAsync(request.ConversationId, userId);
        if (!isMember)
            throw new HubException("You are not a member of this conversation.");

        var sender = await userRepository.GetByIdAsync(userId)
            ?? throw new HubException("User not found.");

        var now = DateTime.UtcNow;
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = userId,
            Content = MessageContent.CreateText(request.Body),
            CreatedAt = now,
            SentAt = now,
            Conversation = null!,
            Sender = sender
        };

        await messageRepository.AddAsync(message);
        await unitOfWork.SaveChangesAsync();

        var response = new MessageResponse(
            message.Id,
            message.ConversationId,
            message.SenderId,
            sender.DisplayName,
            message.Content.Body,
            message.CreatedAt,
            message.SentAt);

        await Clients.Group(request.ConversationId.ToString())
            .SendAsync("ReceiveMessage", response);
    }

    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserId();

        var isMember = await conversationRepository.IsMemberAsync(conversationId, userId);
        if (!isMember)
            throw new HubException("You are not a member of this conversation.");

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    private Guid GetUserId()
    {
        var userIdString = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
