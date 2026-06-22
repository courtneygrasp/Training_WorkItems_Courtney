namespace Training.WorkItems.Domain.Common;

public static class Guard
{
    public static void NotNullOrWhiteSpace(string value, string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null, empty, or white space.", parameterName ?? nameof(value));
        }
    }

    public static void NotEmpty(Guid value, string? parameterName = null)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Value cannot be empty.", parameterName ?? nameof(value));
        }
    }
}
