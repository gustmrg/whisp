using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Whisp.API.Development;
using Whisp.API.Dtos;
using Whisp.API.Hubs;
using Whisp.API.Middlewares;
using Whisp.Application.Interfaces;
using Whisp.Domain.Entities;
using Whisp.Domain.Enums;
using Whisp.Infrastructure;
using Whisp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSignalR();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            }
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Whisp API")
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", auth =>
            {
                auth.Token = string.Empty;
            });
    });

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Retry database connection and migration
    var maxRetries = 10;
    var delaySeconds = 3;

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database and apply migrations (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);
            db.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
            await DevelopmentUserSeeder.SeedAsync(db, logger);
            break;
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            logger.LogWarning(ex, "Failed to connect to database. Retrying in {DelaySeconds} seconds...", delaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

app.UseCors();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapHub<ChatHub>("/hubs/chat");

app.MapGet("/api/conversations/{conversationId:guid}/messages",
    async (Guid conversationId, int skip, int take,
           IMessageRepository messageRepo, IConversationRepository convRepo,
           HttpContext httpContext) =>
    {
        var userIdStr = httpContext.Request.Query["userId"].ToString();
        if (!Guid.TryParse(userIdStr, out var userId))
            return Results.BadRequest("Missing or invalid userId");

        var isMember = await convRepo.IsMemberAsync(conversationId, userId);
        if (!isMember) return Results.Forbid();

        var messages = await messageRepo.GetByConversationIdAsync(conversationId, skip, take);
        var response = messages.Select(m => new MessageResponse(
            m.Id, m.ConversationId, m.SenderId,
            m.Sender.DisplayName, m.Content.Body,
            m.CreatedAt, m.SentAt)).ToList();

        return Results.Ok(response);
    });

app.MapGet("/api/conversations",
    async (IConversationRepository convRepo, IMessageRepository msgRepo,
           HttpContext httpContext) =>
    {
        var userIdStr = httpContext.Request.Query["userId"].ToString();
        if (!Guid.TryParse(userIdStr, out var userId))
            return Results.BadRequest("Missing or invalid userId");

        var conversations = await convRepo.GetByUserIdAsync(userId);
        var response = new List<ConversationResponse>();

        foreach (var conv in conversations)
        {
            var lastMessage = await msgRepo.GetLatestByConversationIdAsync(conv.Id);
            response.Add(new ConversationResponse(
                conv.Id, conv.Type.ToString(), conv.CreatedAt,
                conv.Members.Select(m => new MemberResponse(
                    m.UserId, m.User.DisplayName, m.Role.ToString())).ToList(),
                lastMessage is null ? null : new MessageResponse(
                    lastMessage.Id, lastMessage.ConversationId, lastMessage.SenderId,
                    lastMessage.Sender.DisplayName, lastMessage.Content.Body,
                    lastMessage.CreatedAt, lastMessage.SentAt)));
        }

        return Results.Ok(response);
    });

app.MapPost("/api/conversations",
    async (CreateConversationRequest request,
           IConversationRepository convRepo, IUserRepository userRepo,
           IUnitOfWork unitOfWork, HttpContext httpContext) =>
    {
        var userIdStr = httpContext.Request.Query["userId"].ToString();
        if (!Guid.TryParse(userIdStr, out var userId))
            return Results.BadRequest("Missing or invalid userId");

        var participant = await userRepo.GetByIdAsync(request.ParticipantId);
        if (participant is null)
            return Results.BadRequest("Participant not found");

        var creator = await userRepo.GetByIdAsync(userId);
        if (creator is null)
            return Results.BadRequest("User not found");

        // Return existing direct conversation if one already exists
        var existing = await convRepo.GetDirectConversationAsync(userId, request.ParticipantId);
        if (existing is not null)
        {
            var existingWithMembers = await convRepo.GetByIdWithMembersAsync(existing.Id);
            return Results.Ok(new ConversationResponse(
                existingWithMembers!.Id, existingWithMembers.Type.ToString(), existingWithMembers.CreatedAt,
                existingWithMembers.Members.Select(m => new MemberResponse(
                    m.UserId, m.User.DisplayName, m.Role.ToString())).ToList(),
                null));
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Type = ConversationType.Direct,
            CreatedAt = DateTime.UtcNow
        };

        await convRepo.AddAsync(conversation);

        await convRepo.AddMemberAsync(new ConversationMember
        {
            ConversationId = conversation.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            Role = ConversationRole.Admin,
            Conversation = conversation,
            User = creator
        });

        await convRepo.AddMemberAsync(new ConversationMember
        {
            ConversationId = conversation.Id,
            UserId = request.ParticipantId,
            JoinedAt = DateTime.UtcNow,
            Role = ConversationRole.Member,
            Conversation = conversation,
            User = participant
        });

        await unitOfWork.SaveChangesAsync();

        var response = new ConversationResponse(
            conversation.Id, conversation.Type.ToString(), conversation.CreatedAt,
            [
                new MemberResponse(userId, creator.DisplayName, ConversationRole.Admin.ToString()),
                new MemberResponse(request.ParticipantId, participant.DisplayName, ConversationRole.Member.ToString())
            ],
            null);

        return Results.Created($"/api/conversations/{conversation.Id}", response);
    });

app.MapPost("/api/users",
    async (CreateUserRequest request, IUserRepository userRepo, IUnitOfWork unitOfWork) =>
    {
        const int usernameMaxLength = 30;
        const int displayNameMaxLength = 50;

        var username = request.Username?.Trim() ?? string.Empty;
        var displayName = request.DisplayName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username))
            return Results.BadRequest("Username is required");

        if (string.IsNullOrWhiteSpace(displayName))
            return Results.BadRequest("Display name is required");

        if (username.Length > usernameMaxLength)
            return Results.BadRequest($"Username must be {usernameMaxLength} characters or fewer");

        if (displayName.Length > displayNameMaxLength)
            return Results.BadRequest($"Display name must be {displayNameMaxLength} characters or fewer");

        var usernameExists = await userRepo.ExistsByUsernameAsync(username);
        if (usernameExists)
            return Results.Conflict("Username is already taken");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow
        };

        await userRepo.AddAsync(user);
        try
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Results.Conflict("Username is already taken");
        }

        var response = new UserResponse(user.Id, user.Username, user.DisplayName);
        return Results.Created($"/api/users/{user.Id}", response);
    });

app.MapGet("/api/users",
    async (IUserRepository userRepo, HttpContext httpContext) =>
    {
        var userIdStr = httpContext.Request.Query["userId"].ToString();
        if (!Guid.TryParse(userIdStr, out var userId))
            return Results.BadRequest("Missing or invalid userId");

        var users = await userRepo.GetAllAsync();
        var response = users
            .Where(u => u.Id != userId)
            .OrderBy(u => u.DisplayName)
            .Select(u => new UserResponse(u.Id, u.Username, u.DisplayName))
            .ToList();

        return Results.Ok(response);
    });

app.MapGet("/api/users/search",
    async (string q, IUserRepository userRepo, HttpContext httpContext) =>
    {
        var userIdStr = httpContext.Request.Query["userId"].ToString();
        if (!Guid.TryParse(userIdStr, out var userId))
            return Results.BadRequest("Missing or invalid userId");

        if (string.IsNullOrWhiteSpace(q))
            return Results.Ok(Array.Empty<UserResponse>());

        var users = await userRepo.SearchByUsernameAsync(q);
        var response = users
            .Where(u => u.Id != userId)
            .Select(u => new UserResponse(u.Id, u.Username, u.DisplayName))
            .ToList();

        return Results.Ok(response);
    });

app.Run();
