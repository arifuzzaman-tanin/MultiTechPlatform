using MultiTech.Platform.Application.Common.Results;

namespace MultiTech.Platform.Application.Abstractions.Messaging;

/// <summary>
/// Handles a query.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handles the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The operation result.</returns>
    Task<Result<TResponse>> Handle(
        TQuery query,
        CancellationToken cancellationToken);
}

