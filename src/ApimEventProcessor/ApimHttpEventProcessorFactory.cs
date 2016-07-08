using Microsoft.ServiceBus.Messaging;

namespace ApimEventProcessor
{
    /// <summary>
    ///  Allows the EventProcessor instances to have services injected into the constructor
    /// </summary>
    public class ApimHttpEventProcessorFactory : IEventProcessorFactory
    {
        private readonly IHttpMessageProcessor httpMessageProcessor;
        private readonly ILogger logger;
        private readonly IMessageFilter messageFilter;

        public ApimHttpEventProcessorFactory(IHttpMessageProcessor httpMessageProcessor, ILogger logger, IMessageFilter messageFilter)
        {
            this.httpMessageProcessor = httpMessageProcessor;
            this.logger = logger;
            this.messageFilter = messageFilter;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new ApimEventProcessor(httpMessageProcessor, logger, messageFilter);
        }
    }
}