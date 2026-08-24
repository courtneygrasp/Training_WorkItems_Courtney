using AwesomeAssertions;
using Training.WorkItems.Domain.WorkItems.Entities;
using Training.WorkItems.Domain.WorkItems.Enums;
using Training.WorkItems.Domain.WorkItems.ValueTypes;
using Training.WorkItems.Infrastructure.WorkItems.Repositories;
using Training.WorkItems.Infrastructure.WorkItems.Services;
using NoteRepo = Training.WorkItems.Tests.Application.WorkItems.Fakes.InMemoryWorkItemNoteRepository;

namespace Training.WorkItems.Tests.Infrastructure;

public sealed class WorkItemInfrastructureTests
{
    // InMemoryWorkItemRepository

    [Fact]
    public async Task InMemoryRepository_AddAsync_StoresWorkItem()
    {
        var repo = new InMemoryWorkItemRepository();
        var workItem = CreateWorkItem(TenantId.Create(Guid.NewGuid()), "Test Item");

        await repo.AddAsync(workItem, CancellationToken.None);

        var found = await repo.GetByIdAsync(workItem.TenantId, workItem.Id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.Id.Should().Be(workItem.Id);
    }

    [Fact]
    public async Task InMemoryRepository_GetByIdAsync_WhenTenantDiffers_ReturnsNull()
    {
        var repo = new InMemoryWorkItemRepository();
        var workItem = CreateWorkItem(TenantId.Create(Guid.NewGuid()), "Test Item");
        await repo.AddAsync(workItem, CancellationToken.None);

        var found = await repo.GetByIdAsync(TenantId.Create(Guid.NewGuid()), workItem.Id, CancellationToken.None);

        found.Should().BeNull();
    }

    [Fact]
    public async Task InMemoryRepository_ListAsync_ReturnsOnlyMatchingTenantItems()
    {
        var repo = new InMemoryWorkItemRepository();
        var tenantA = TenantId.Create(Guid.NewGuid());
        var tenantB = TenantId.Create(Guid.NewGuid());

        await repo.AddAsync(CreateWorkItem(tenantA, "A1"), CancellationToken.None);
        await repo.AddAsync(CreateWorkItem(tenantA, "A2"), CancellationToken.None);
        await repo.AddAsync(CreateWorkItem(tenantB, "B1"), CancellationToken.None);

        var results = await repo.ListAsync(tenantA, CancellationToken.None);

        results.Should().HaveCount(2);
        results.Should().AllSatisfy(x => x.TenantId.Should().Be(tenantA));
    }

    [Fact]
    public async Task InMemoryRepository_UpdateAsync_CompletesWithoutError()
    {
        var repo = new InMemoryWorkItemRepository();
        var workItem = CreateWorkItem(TenantId.Create(Guid.NewGuid()), "Test Item");
        await repo.AddAsync(workItem, CancellationToken.None);

        var act = async () => await repo.UpdateAsync(workItem, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    // DefaultCurrentUserContext

    [Fact]
    public void DefaultCurrentUserContext_ReturnsExpectedTenantId()
    {
        var context = new DefaultCurrentUserContext();

        context.TenantId.Value.Should().Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }

    [Fact]
    public void DefaultCurrentUserContext_ReturnsExpectedUserId()
    {
        var context = new DefaultCurrentUserContext();

        context.UserId.Should().Be(Guid.Parse("22222222-2222-2222-2222-222222222222"));
    }

    [Fact]
    public void DefaultCurrentUserContext_CanCloseWorkItems_IsTrue()
    {
        var context = new DefaultCurrentUserContext();

        context.CanCloseWorkItems.Should().BeTrue();
    }

    // SystemClock

    [Fact]
    public void SystemClock_UtcNow_ReturnsCurrentTime()
    {
        var clock = new SystemClock();
        var before = DateTimeOffset.UtcNow;

        var result = clock.UtcNow;

        result.Should().BeOnOrAfter(before);
    }

    // WorkItemStorageMappings

    [Fact]
    public void WorkItemStorageMappings_ToStorageRecord_MapsAllFields()
    {
        var workItem = CreateWorkItem(TenantId.Create(Guid.NewGuid()), "Fix login bug");
        workItem.ChangeDescription("Some description");

        var record = workItem.ToStorageRecord();

        record.WorkItemId.Should().Be(workItem.Id.Value);
        record.TenantId.Should().Be(workItem.TenantId.Value);
        record.Title.Should().Be("Fix login bug");
        record.Description.Should().Be("Some description");
        record.Status.Should().Be("New");
        record.CreatedAt.Should().Be(workItem.CreatedAt);
    }

    [Fact]
    public void WorkItemStorageMappings_ToDomain_RehydratesWorkItem()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var record = new WorkItemStorageRecord
        {
            WorkItemId = id,
            TenantId = tenantId,
            Title = "Fix login bug",
            Description = "Users cannot sign in.",
            Status = "InProgress",
            CreatedAt = createdAt
        };

        var workItem = record.ToDomain();

        workItem.Id.Value.Should().Be(id);
        workItem.TenantId.Value.Should().Be(tenantId);
        workItem.Title.Value.Should().Be("Fix login bug");
        workItem.Description.Should().Be("Users cannot sign in.");
        workItem.Status.Should().Be(WorkItemStatus.InProgress);
        workItem.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void WorkItemStorageMappings_ToDomain_WithNullDescription_RehydratesWithNullDescription()
    {
        var record = new WorkItemStorageRecord
        {
            WorkItemId = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Title = "Fix login bug",
            Description = null,
            Status = "New",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var workItem = record.ToDomain();

        workItem.Description.Should().BeNull();
    }

    private static WorkItem CreateWorkItem(TenantId tenantId, string title) =>
        WorkItem.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            WorkItemTitle.Create(title),
            null,
            DateTimeOffset.UtcNow);

    // InMemoryWorkItemNoteRepository

    [Fact]
    public async Task InMemoryNoteRepository_AddAsync_StoresNote()
    {
        var repo = new NoteRepo();
        var note = CreateNote(TenantId.Create(Guid.NewGuid()), "Test note.");

        await repo.AddAsync(note, CancellationToken.None);

        repo.Notes.Should().ContainSingle()
            .Which.Id.Should().Be(note.Id);
    }

    // WorkItemNoteStorageMappings

    [Fact]
    public void WorkItemNoteStorageMappings_ToStorageRecord_MapsAllFields()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var workItemId = WorkItemId.Create(Guid.NewGuid());
        var authorId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var note = WorkItemNote.Create(
            workItemId,
            tenantId,
            authorId,
            WorkItemNoteText.Create("Fix the login page."),
            createdAt);

        var record = note.ToStorageRecord();

        record.NoteId.Should().Be(note.Id);
        record.WorkItemId.Should().Be(workItemId.Value);
        record.TenantId.Should().Be(tenantId.Value);
        record.AuthorId.Should().Be(authorId);
        record.NoteText.Should().Be("Fix the login page.");
        record.CreatedAt.Should().Be(createdAt);
    }

    private static WorkItemNote CreateNote(TenantId tenantId, string noteText) =>
        WorkItemNote.Create(
            WorkItemId.Create(Guid.NewGuid()),
            tenantId,
            Guid.NewGuid(),
            WorkItemNoteText.Create(noteText),
            DateTimeOffset.UtcNow);

    // WorkItemAuditStorageMappings

    [Fact]
    public void WorkItemAuditStorageMappings_ToStorageRecord_MapsAllFields()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var workItem = CreateWorkItem(tenantId, "Fix login bug");
        workItem.ChangeStatus(WorkItemStatus.InProgress, new Training.WorkItems.Domain.WorkItems.Services.DefaultWorkItemStatusPolicy());
        var auditRecord = WorkItemAuditRecord.Create(
            workItem.Id,
            workItem.TenantId,
            Guid.NewGuid(),
            workItem.Status);

        var record = auditRecord.ToStorageRecord();

        record.AuditId.Should().Be(auditRecord.Id);
        record.WorkItemId.Should().Be(auditRecord.WorkItemId.Value);
        record.TenantId.Should().Be(auditRecord.TenantId.Value);
        record.UserId.Should().Be(auditRecord.UserId);
        record.Status.Should().Be("InProgress");
        record.ChangedAt.Should().Be(auditRecord.ChangedAt);
    }
}
