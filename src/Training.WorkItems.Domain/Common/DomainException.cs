namespace Training.WorkItems.Domain.Common;

public abstract class DomainException : Exception
{
    protected DomainException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }

    protected void AddData(string key, string value)
    {
        Data[key] = value;
    }
}
