using Microsoft.AspNetCore.SignalR;
using Whisp.API.Dtos;
using Whisp.Application.Interfaces;
using Whisp.Domain.Entities;
using Whisp.Domain.Enums;
using Whisp.Domain.ValueObjects;

namespace Whisp.API.Hubs;

public class ChatHub(
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<ChatHub> logger) : Hub
{
    private const string UserGroupPrefix = "user:";

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));

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

    public async Task<SendDirectMessageResponse> SendDirectMessage(SendDirectMessageRequest request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            throw new HubException("Missing or invalid userId.");

        if (request.ParticipantId == userId)
            throw new HubException("You cannot message yourself.");

        var sender = await userRepository.GetByIdAsync(userId)
            ?? throw new HubException("User not found.");

        var participant = await userRepository.GetByIdAsync(request.ParticipantId)
            ?? throw new HubException("Participant not found.");

        var conversation = await conversationRepository.GetDirectConversationAsync(userId, request.ParticipantId);
        var conversationCreated = false;

        if (conversation is null)
        {
            conversationCreated = true;
            var now = DateTime.UtcNow;

            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Direct,
                CreatedAt = now
            };

            await conversationRepository.AddAsync(conversation);

            await conversationRepository.AddMemberAsync(new ConversationMember
            {
                ConversationId = conversation.Id,
                UserId = userId,
                JoinedAt = now,
                Role = ConversationRole.Admin,
                Conversation = conversation,
                User = sender
            });

            await conversationRepository.AddMemberAsync(new ConversationMember
            {
                ConversationId = conversation.Id,
                UserId = request.ParticipantId,
                JoinedAt = now,
                Role = ConversationRole.Member,
                Conversation = conversation,
                User = participant
            });
        }

        var createdAt = DateTime.UtcNow;
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = userId,
            Content = MessageContent.CreateText(request.Body),
            CreatedAt = createdAt,
            SentAt = createdAt,
            Conversation = conversation,
            Sender = sender
        };

        await messageRepository.AddAsync(message);
        await unitOfWork.SaveChangesAsync();

        await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());

        var messageResponse = new MessageResponse(
            message.Id,
            message.ConversationId,
            message.SenderId,
            sender.DisplayName,
            message.Content.Body,
            message.CreatedAt,
            message.SentAt);

        if (conversationCreated)
        {
            var conversationCreatedEvent = new ConversationCreatedResponse(conversation.Id);
            await Clients.Group(GetUserGroupName(request.ParticipantId))
                .SendAsync("ConversationCreated", conversationCreatedEvent);
            await Clients.Group(GetUserGroupName(userId))
                .SendAsync("ConversationCreated", conversationCreatedEvent);
        }

        await Clients.Group(conversation.Id.ToString())
            .SendAsync("ReceiveMessage", messageResponse);

        return new SendDirectMessageResponse(conversation.Id, messageResponse, conversationCreated);
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

    private static string GetUserGroupName(Guid userId) => $"{UserGroupPrefix}{userId}";
}
