namespace ApimEventProcessor
{
    public interface IMessageFilter
    {
        bool ShouldProcess(HttpMessage message);
    }
}