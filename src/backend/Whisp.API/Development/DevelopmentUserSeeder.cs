using Microsoft.EntityFrameworkCore;
using Whisp.Domain.Entities;
using Whisp.Infrastructure.Persistence;

namespace Whisp.API.Development;

public static class DevelopmentUserSeeder
{
    private static readonly SeedUser[] SeedUsers =
    [
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            "dev_alice",
            "Dev Alice"),
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000002"),
            "dev_bob",
            "Dev Bob"),
        new(
            Guid.Parse("00000000-0000-0000-0000-000000000003"),
            "dev_carla",
            "Dev Carla")
    ];

    public static async Task SeedAsync(AppDbContext db, ILogger logger, CancellationToken ct = default)
    {
        var seedIds = SeedUsers.Select(u => u.Id).ToHashSet();
        var seedUsernames = SeedUsers.Select(u => u.Username).ToHashSet(StringComparer.Ordinal);

        var existingUsers = await db.Users
            .Where(u => seedIds.Contains(u.Id) || seedUsernames.Contains(u.Username))
            .ToListAsync(ct);

        var existingById = existingUsers.ToDictionary(u => u.Id);
        var existingByUsername = existingUsers
            .GroupBy(u => u.Username, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

        var createdCount = 0;
        var updatedCount = 0;
        var skippedCount = 0;

        foreach (var seed in SeedUsers)
        {
            if (existingById.TryGetValue(seed.Id, out var byId))
            {
                if (existingByUsername.TryGetValue(seed.Username, out var usernameOwner)
                    && usernameOwner.Id != byId.Id)
                {
                    skippedCount++;
                    logger.LogWarning(
                        "Skipping dev seed user {SeedUserId} ({SeedUsername}): username already belongs to {ExistingUserId}",
                        seed.Id,
                        seed.Username,
                        usernameOwner.Id);
                    continue;
                }

                var changed = false;
                var oldUsername = byId.Username;

                if (byId.Username != seed.Username)
                {
                    byId.Username = seed.Username;
                    changed = true;
                }

                if (byId.DisplayName != seed.DisplayName)
                {
                    byId.DisplayName = seed.DisplayName;
                    changed = true;
                }

                if (changed)
                {
                    updatedCount++;
                    existingByUsername.Remove(oldUsername);
                    existingByUsername[seed.Username] = byId;
                }

                continue;
            }

            if (existingByUsername.TryGetValue(seed.Username, out var byUsername)
                && byUsername.Id != seed.Id)
            {
                skippedCount++;
                logger.LogWarning(
                    "Skipping dev seed user {SeedUserId} ({SeedUsername}): username already belongs to {ExistingUserId}",
                    seed.Id,
                    seed.Username,
                    byUsername.Id);
                continue;
            }

            var newUser = new User
            {
                Id = seed.Id,
                Username = seed.Username,
                DisplayName = seed.DisplayName,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(newUser);
            existingById[seed.Id] = newUser;
            existingByUsername[seed.Username] = newUser;
            createdCount++;
        }

        if (createdCount > 0 || updatedCount > 0)
        {
            await db.SaveChangesAsync(ct);
        }

        logger.LogInformation(
            "Development user seeding completed. Created: {CreatedCount}, Updated: {UpdatedCount}, Skipped: {SkippedCount}",
            createdCount,
            updatedCount,
            skippedCount);
    }

    private sealed record SeedUser(Guid Id, string Username, string DisplayName);
}
