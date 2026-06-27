namespace Training.WorkItems.Application.Common;

public sealed class ApplicationResult<T>
{
    private ApplicationResult(bool succeeded, T? value, string? errorMessage)
    {
        Succeeded = succeeded;
        Value = value;
        ErrorMessage = errorMessage;
    }

    public bool Succeeded { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }

    public static ApplicationResult<T> Success(T value) => new(true, value, null);

    public static ApplicationResult<T> Invalid(string errorMessage) => new(false, default, errorMessage);
}
