using Training.WorkItems.Domain.WorkItems.ValueTypes;

namespace Training.WorkItems.Domain.WorkItems.Entities;

/// <summary>Represents a text note attached to a work item.</summary>
public sealed class WorkItemNote
{
    private WorkItemNote(
        Guid id,
        WorkItemId workItemId,
        TenantId tenantId,
        Guid authorId,
        WorkItemNoteText noteText,
        DateTimeOffset createdAt)
    {
        Id = id;
        WorkItemId = workItemId;
        TenantId = tenantId;
        AuthorId = authorId;
        NoteText = noteText;
        CreatedAt = createdAt;
    }

    /// <summary>Gets the unique identifier for this note.</summary>
    public Guid Id { get; }

    /// <summary>Gets the work item this note belongs to.</summary>
    public WorkItemId WorkItemId { get; }

    /// <summary>Gets the tenant that owns the work item.</summary>
    public TenantId TenantId { get; }

    /// <summary>Gets the ID of the user who authored the note.</summary>
    public Guid AuthorId { get; }

    /// <summary>Gets the validated text content of the note.</summary>
    public WorkItemNoteText NoteText { get; }

    /// <summary>Gets the UTC time the note was created.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Creates a new note for the specified work item.</summary>
    public static WorkItemNote Create(
        WorkItemId workItemId,
        TenantId tenantId,
        Guid authorId,
        WorkItemNoteText noteText,
        DateTimeOffset createdAt) =>
        new(Guid.NewGuid(), workItemId, tenantId, authorId, noteText, createdAt);
}
