using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Tests.Application.Time;

public sealed class FakeSystemClock(DateTimeOffset utcNow) : ISystemClock
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}
