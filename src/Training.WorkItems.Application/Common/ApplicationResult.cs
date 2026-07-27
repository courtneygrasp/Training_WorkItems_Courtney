using Training.WorkItems.Domain.Common.Validation;

namespace Training.WorkItems.Application.Common;

public enum ResultStatus
{
    Success,
    Invalid,
    NotFound,
    Forbidden
}

public sealed class ApplicationResult<T>
{
    private ApplicationResult(ResultStatus status, T? value, string? errorMessage, IReadOnlyList<ValidationFailure>? failures)
    {
        Status = status;
        Value = value;
        ErrorMessage = errorMessage;
        Failures = failures ?? [];
    }

    public ResultStatus Status { get; }
    public bool Succeeded => Status == ResultStatus.Success;
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<ValidationFailure> Failures { get; }

    public static ApplicationResult<T> Success(T value) => new(ResultStatus.Success, value, null, null);

    public static ApplicationResult<T> Invalid(string errorMessage) =>
        new(ResultStatus.Invalid, default, errorMessage, null);

    public static ApplicationResult<T> Invalid(IReadOnlyList<ValidationFailure> failures) =>
        new(ResultStatus.Invalid, default, null, failures);

    public static ApplicationResult<T> NotFound() =>
        new(ResultStatus.NotFound, default, null, null);

    public static ApplicationResult<T> Forbidden() =>
        new(ResultStatus.Forbidden, default, null, null);
}
