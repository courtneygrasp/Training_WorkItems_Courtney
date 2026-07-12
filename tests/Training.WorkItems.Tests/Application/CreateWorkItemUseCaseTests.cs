using AwesomeAssertions;
using Training.WorkItems.Application.WorkItems.UseCases;
using Training.WorkItems.Domain.WorkItems.ValueTypes;
using Training.WorkItems.Tests.Application.Time;
using Training.WorkItems.Tests.Application.WorkItems.Fakes;

namespace Training.WorkItems.Tests.Application;

public sealed class CreateWorkItemUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCommand_CreatesAndSavesWorkItem()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var createdAt = new DateTimeOffset(2026, 6, 26, 10, 30, 0, TimeSpan.Zero);
        var useCase = CreateUseCase(repository, tenantId, createdAt);
        var command = new CreateWorkItemCommand("Fix login bug", "Users cannot sign in.");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Fix login bug");
        result.Value.Description.Should().Be("Users cannot sign in.");
        result.Value.Status.Should().Be("New");
        result.Value.CreatedAt.Should().Be(createdAt);
        repository.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task ExecuteAsync_WithBlankTitle_ReturnsInvalidResult()
    {
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository);
        var command = new CreateWorkItemCommand(" ", "Description");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        repository.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_UsesCurrentTenantForSavedWorkItem()
    {
        var tenantId = TenantId.Create(Guid.NewGuid());
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository, tenantId);
        var command = new CreateWorkItemCommand("Fix login bug", null);

        await useCase.ExecuteAsync(command, CancellationToken.None);

        repository.Items.Should().ContainSingle();
        repository.Items.Single().TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryReceivesCreatedWorkItem()
    {
        var repository = new InMemoryWorkItemRepository();
        var useCase = CreateUseCase(repository);
        var command = new CreateWorkItemCommand("Fix login bug", "Description");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        repository.Items.Should().ContainSingle();
        repository.Items.Single().Id.Value.Should().Be(result.Value!.Id);
        repository.Items.Single().Title.Value.Should().Be("Fix login bug");
    }

    private static CreateWorkItemUseCase CreateUseCase(
        InMemoryWorkItemRepository repository,
        TenantId? tenantId = null,
        DateTimeOffset? utcNow = null)
    {
        return new CreateWorkItemUseCase(
            repository,
            new FakeCurrentUserContext(tenantId ?? TenantId.Create(Guid.NewGuid())),
            new FakeSystemClock(utcNow ?? DateTimeOffset.UtcNow));
    }
}
