namespace MultiTech.Platform.Application.Abstractions.Messaging;

/// <summary>
/// Represents a command.
/// </summary>
public interface ICommand;

/// <summary>
/// Represents a command that returns a value.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommand<TResponse>;

