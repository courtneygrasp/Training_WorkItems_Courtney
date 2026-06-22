using Training.WorkItems.Domain.Common;

namespace Training.WorkItems.Domain.WorkItems.ValueTypes;

public readonly record struct WorkItemId
{
    private WorkItemId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static WorkItemId Create(Guid value)
    {
        Guard.NotEmpty(value);

        return new WorkItemId(value);
    }

    public override string ToString() => Value.ToString();
}
