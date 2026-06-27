namespace Training.WorkItems.Application.Common;

public interface ISystemClock
{
    DateTimeOffset UtcNow { get; }
}
