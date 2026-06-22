using Training.WorkItems.Domain.Common;

namespace Training.WorkItems.Domain.WorkItems.ValueTypes;

public readonly record struct TenantId
{
    private TenantId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static TenantId Create(Guid value)
    {
        Guard.NotEmpty(value);

        return new TenantId(value);
    }

    public override string ToString() => Value.ToString();
}
