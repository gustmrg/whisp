using Whisp.Domain.Entities;

namespace Whisp.Application.Interfaces;

/// <summary>
/// Repository interface for <see cref="User"/> entity operations.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user if found; otherwise, <c>null</c>.</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a user with the specified username exists.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if a user with the username exists; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for users whose username matches the specified query.
    /// </summary>
    /// <param name="query">The search query to match against usernames.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of matching users.</returns>
    Task<IReadOnlyList<User>> SearchByUsernameAsync(string query, CancellationToken cancellationToken = default);
}
