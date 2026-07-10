namespace Training.WorkItems.Domain.Common.Validation;

public interface IValidator<TContext>
{
    ValidationResult Validate(TContext context);
}
