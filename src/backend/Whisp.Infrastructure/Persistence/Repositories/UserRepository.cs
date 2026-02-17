using Microsoft.EntityFrameworkCore;
using Whisp.Application.Interfaces;
using Whisp.Domain.Entities;

namespace Whisp.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> SearchByUsernameAsync(string query, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(u => EF.Functions.ILike(u.Username, $"%{query}%"))
            .ToListAsync(cancellationToken);
    }
}
