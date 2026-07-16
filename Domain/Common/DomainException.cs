namespace Domain.Common;

/// <summary>
/// Thrown when an operation would violate a domain invariant — a rule that
/// must always hold true regardless of caller (e.g. price must be positive,
/// VIN must be well-formed, a suspended listing can't be republished
/// directly). This is distinct from framework-level exceptions
/// (ArgumentNullException, InvalidOperationException) so the application
/// layer can catch domain rule violations specifically and translate them
/// into a client-facing error (e.g. HTTP 400) without conflating them with
/// genuine bugs or infrastructure failures, which should surface as 500s
/// and get logged/alerted differently.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}