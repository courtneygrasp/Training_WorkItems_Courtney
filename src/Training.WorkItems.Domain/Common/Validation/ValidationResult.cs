namespace Training.WorkItems.Domain.Common.Validation;

public sealed class ValidationResult
{
    private ValidationResult(bool succeeded, IReadOnlyList<ValidationFailure> failures)
    {
        Succeeded = succeeded;
        Failures = failures;
    }

    public bool Succeeded { get; }
    public IReadOnlyList<ValidationFailure> Failures { get; }

    public static ValidationResult Success() => new(true, []);

    public static ValidationResult Failure(IEnumerable<ValidationFailure> failures) =>
        new(false, failures.ToList());
}
