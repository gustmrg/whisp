using Whisp.Domain.Entities;

namespace Whisp.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> SearchByUsernameAsync(string query, CancellationToken cancellationToken = default);
}
