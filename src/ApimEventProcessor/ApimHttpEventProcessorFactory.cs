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

        public ApimHttpEventProcessorFactory(IHttpMessageProcessor httpMessageProcessor, ILogger logger)
        {
            this.httpMessageProcessor = httpMessageProcessor;
            this.logger = logger;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new ApimEventProcessor(httpMessageProcessor, logger);
        }
    }
}