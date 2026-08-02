using Training.WorkItems.Domain.Common;

namespace Training.WorkItems.Domain.WorkItems.Exceptions;

/// <summary>
/// Thrown when a status change request violates a cross-input workflow rule.
/// This exception is caller-safe: its message and error code may be surfaced to the caller.
/// </summary>
public sealed class WorkItemStatusChangeValidationException : DomainException
{
    /// <summary>
    /// Initializes a new instance with the failing rule's error code and the request property that triggered it.
    /// </summary>
    public WorkItemStatusChangeValidationException(string errorCode, string target)
        : base(errorCode, "The work item status change request is invalid.")
    {
        AddData(nameof(target), target);
    }
}
