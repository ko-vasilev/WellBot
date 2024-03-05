﻿// ReSharper disable once IdentifierTypo
namespace MediatR;

/// <summary>
/// Wrapper class for a handler that asynchronously handles a request and does not return a response.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
public abstract class AsyncRequestHandler<TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit>
{
    /// <inheritdoc />
    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }

    /// <summary>
    /// Override in a derived class for the handler logic.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response.</returns>
    protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}
