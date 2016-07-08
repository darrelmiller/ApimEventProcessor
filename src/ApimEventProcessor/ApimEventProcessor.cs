using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApimEventProcessor
{
    /// <summary>
    /// Accepts EventData from EventHubs, converts to a HttpMessage instances and forwards it to a IHttpMessageProcessor
    /// </summary>
    public class ApimEventProcessor : IEventProcessor
    {
        private Stopwatch checkpointStopWatch;
        private readonly ILogger logger;
        private readonly IMessageFilter messageFilter;
        private readonly IHttpMessageProcessor messageContentProcessor;
        private readonly Dictionary<Guid, HttpMessage> requestMessages = new Dictionary<Guid, HttpMessage>();

        public ApimEventProcessor(IHttpMessageProcessor messageContentProcessor, ILogger logger, IMessageFilter messageFilter)
        {
            this.messageContentProcessor = messageContentProcessor;
            this.logger = logger;
            this.messageFilter = messageFilter;
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                //logger.LogInfo($"Event received from partition: {context.Lease.PartitionId} - {eventData.SequenceNumber}");

                try
                {
                    var httpMessage = await HttpMessage.Parse(eventData.GetBodyStream());

                    if (httpMessage.IsRequest)
                    {
                        requestMessages.Add(httpMessage.MessageId, httpMessage);
                    }
                    else
                    {
                        HttpMessage requestMessage;
                        if (requestMessages.TryGetValue(httpMessage.MessageId, out requestMessage))
                        {
                            requestMessages.Remove(httpMessage.MessageId);
                            httpMessage.HttpRequestMessage = requestMessage.HttpRequestMessage;

                            if (messageFilter.ShouldProcess(httpMessage))
                            {
                                await messageContentProcessor.ProcessHttpMessage(httpMessage);
                                logger.LogInfo("Processed: " + httpMessage.HttpRequestMessage.RequestUri);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }

            //Call checkpoint every 1 minute, so that worker can resume processing from the 1 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(1))
            {
                logger.LogInfo("Checkpointing");
                logger.LogInfo($"Cache size is: {requestMessages.Count}");
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            logger.LogInfo("Processor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            logger.LogDebug("SimpleEventProcessor initialized.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset);
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }
    }
}