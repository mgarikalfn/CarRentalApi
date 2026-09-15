namespace Application.Common;

/// <summary>
/// Generic result type for representing success or failure with optional value/error details.
/// Used throughout the application layer to communicate operation outcomes from handlers and repositories.
/// </summary>
/// <typeparam name="T">The type of value returned on success.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the value returned by a successful operation. Throws if operation failed.
    /// </summary>
    public T Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException($"Cannot access Value on a failed result. Error: {Error}");
            return _value!;
        }
    }

    /// <summary>
    /// Gets the error message from a failed operation.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Gets an optional error code from a failed operation.
    /// </summary>
    public string? ErrorCode { get; private set; }

    private T? _value;

    private Result(bool isSuccess, T? value, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null, null);
    }

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result<T> Failure(string error, string? errorCode = null)
    {
        return new Result<T>(false, default, error, errorCode);
    }
}
