namespace Whisp.Application.Interfaces;

/// <summary>
/// Represents a unit of work for coordinating transactional persistence.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
