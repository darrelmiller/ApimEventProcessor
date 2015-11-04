using System;
using Microsoft.ServiceBus.Messaging;
using System.Net.Http;

namespace ApimEventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string eventHubConnectionString = Environment.GetEnvironmentVariable("APIMEVENTS-EVENTHUB-CONNECTIONSTRING", EnvironmentVariableTarget.User); 
            string eventHubName = Environment.GetEnvironmentVariable("APIMEVENTS-EVENTHUB-NAME", EnvironmentVariableTarget.User); 
            string storageAccountName = Environment.GetEnvironmentVariable("APIMEVENTS-STORAGEACCOUNT-NAME", EnvironmentVariableTarget.User); 
            string storageAccountKey = Environment.GetEnvironmentVariable("APIMEVENTS-STORAGEACCOUNT-KEY", EnvironmentVariableTarget.User);

            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            var eventProcessorHost = new EventProcessorHost(
                                                eventProcessorHostName,
                                                eventHubName,
                                                EventHubConsumerGroup.DefaultGroupName,
                                                eventHubConnectionString,
                                                storageConnectionString);


            var logger = new ConsoleLogger(LogLevel.Debug);
            logger.LogDebug("Registering EventProcessor...");

            var httpMessageProcessor = new RunscopeHttpMessageProcessor(new HttpClient(), logger);

            eventProcessorHost.RegisterEventProcessorFactoryAsync(
                new ApimHttpEventProcessorFactory(httpMessageProcessor, logger));
            
            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }

      
        
    }

 

}
