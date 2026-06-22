using Training.WorkItems.Domain.Common;

namespace Training.WorkItems.Domain.WorkItems.ValueTypes;

public readonly record struct WorkItemTitle
{
    private WorkItemTitle(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static WorkItemTitle Create(string value)
    {
        Guard.NotNullOrWhiteSpace(value);

        if (value.Length > 120)
        {
            throw new ArgumentException("Work item title cannot exceed 120 characters.", nameof(value));
        }

        return new WorkItemTitle(value);
    }

    public override string ToString() => Value;
}
