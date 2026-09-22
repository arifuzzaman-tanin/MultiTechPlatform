using MultiTech.Platform.Application.Common.Results;

namespace MultiTech.Platform.Application.Abstractions.Messaging;

/// <summary>
/// Handles a command.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The operation result.</returns>
    Task<Result<TResponse>> Handle(
        TCommand command,
        CancellationToken cancellationToken);
}

/// <summary>
/// Handles a command without a response value.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The operation result.</returns>
    Task<Result> Handle(
        TCommand command,
        CancellationToken cancellationToken);
}

