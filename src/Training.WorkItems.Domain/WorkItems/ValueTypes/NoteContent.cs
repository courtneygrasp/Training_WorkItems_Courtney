using Training.WorkItems.Domain.Common;

namespace Training.WorkItems.Domain.WorkItems.ValueTypes;

public readonly record struct NoteContent
{
    private NoteContent(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static NoteContent Create(string value)
    {
        Guard.NotNullOrWhiteSpace(value);

        if (value.Length > 2000)
        {
            throw new ArgumentException("Note content cannot exceed 2000 characters.", nameof(value));
        }

        return new NoteContent(value);
    }

    public override string ToString() => Value;
}
