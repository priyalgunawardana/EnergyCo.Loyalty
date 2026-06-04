namespace EnergyCo.Loyalty.Application.Common;

public enum ErrorKind { None, NotFound, Validation, Conflict }

/// <summary>Represents the outcome of an application operation.</summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public ErrorKind ErrorKind { get; }

    protected Result(bool isSuccess, string? error, ErrorKind kind)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorKind = kind;
    }

    public static Result Success() => new(true, null, ErrorKind.None);
    public static Result Failure(string error, ErrorKind kind = ErrorKind.Validation) => new(false, error, kind);
    public static Result<T> Success<T>(T value) => new(value, true, null, ErrorKind.None);
    public static Result<T> Failure<T>(string error, ErrorKind kind = ErrorKind.Validation) => new(default, false, error, kind);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    internal Result(T? value, bool isSuccess, string? error, ErrorKind kind)
        : base(isSuccess, error, kind)
    {
        Value = value;
    }
}
