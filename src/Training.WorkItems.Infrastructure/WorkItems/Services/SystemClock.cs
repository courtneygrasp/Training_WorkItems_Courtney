using Training.WorkItems.Application.Common;

namespace Training.WorkItems.Infrastructure.WorkItems.Services;

public sealed class SystemClock : ISystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
