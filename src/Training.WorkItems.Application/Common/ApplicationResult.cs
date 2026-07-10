using Training.WorkItems.Domain.Common.Validation;

namespace Training.WorkItems.Application.Common;

public sealed class ApplicationResult<T>
{
    private ApplicationResult(bool succeeded, T? value, string? errorMessage, IReadOnlyList<ValidationFailure>? failures)
    {
        Succeeded = succeeded;
        Value = value;
        ErrorMessage = errorMessage;
        Failures = failures ?? [];
    }

    public bool Succeeded { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<ValidationFailure> Failures { get; }

    public static ApplicationResult<T> Success(T value) => new(true, value, null, null);

    public static ApplicationResult<T> Invalid(string errorMessage) => new(false, default, errorMessage, null);

    public static ApplicationResult<T> Invalid(IReadOnlyList<ValidationFailure> failures) =>
        new(false, default, null, failures);
}
