using Training.WorkItems.Domain.Common;

namespace Training.WorkItems.Domain.WorkItems.ValueTypes;

/// <summary>Represents validated text content for a work item note.</summary>
public readonly record struct WorkItemNoteText
{
    private WorkItemNoteText(string value)
    {
        Value = value;
    }

    /// <summary>Gets the raw text value.</summary>
    public string Value { get; }

    /// <summary>Creates a <see cref="WorkItemNoteText"/> from the supplied string, enforcing the length invariant.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, whitespace, or exceeds 2000 characters.</exception>
    public static WorkItemNoteText Create(string value)
    {
        Guard.NotNullOrWhiteSpace(value);

        if (value.Length > 2000)
        {
            throw new ArgumentException("Note text cannot exceed 2000 characters.", nameof(value));
        }

        return new WorkItemNoteText(value);
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
