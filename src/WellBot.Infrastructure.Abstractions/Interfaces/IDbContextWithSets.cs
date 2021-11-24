using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WellBot.Infrastructure.Abstractions.Interfaces
{
    /// <summary>
    /// Database context that can retrieve entities collection by providing type.
    /// </summary>
    public interface IDbContextWithSets
    {
        /// <summary>
        /// Get set of entities by type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Set of entities.</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <summary>
        /// Save pending changes.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
