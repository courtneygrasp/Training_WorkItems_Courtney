namespace Training.WorkItems.Domain.Common.Validation;

public sealed record ValidationFailure(
    string ErrorCode,
    string PropertyName,
    object? AttemptedValue,
    string? Message);
