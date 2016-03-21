using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using System.Net.Http;

namespace ApimEventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            //var eventHubConnectionString = Environment.GetEnvironmentVariable("APIMEVENTS-EVENTHUB-CONNECTIONSTRING", EnvironmentVariableTarget.Process); 
            //var eventHubName = Environment.GetEnvironmentVariable("APIMEVENTS-EVENTHUB-NAME", EnvironmentVariableTarget.Process); 
            //var storageAccountName = Environment.GetEnvironmentVariable("APIMEVENTS-STORAGEACCOUNT-NAME", EnvironmentVariableTarget.Process); 
            //var storageAccountKey = Environment.GetEnvironmentVariable("APIMEVENTS-STORAGEACCOUNT-KEY", EnvironmentVariableTarget.Process);

            var eventHubConnectionString = ConfigurationManager.AppSettings.Get("APIMEVENTS-EVENTHUB-CONNECTIONSTRING");
            var eventHubName = ConfigurationManager.AppSettings.Get("APIMEVENTS-EVENTHUB-NAME");
            var storageConnectionString = ConfigurationManager.AppSettings.Get("APIMEVENTS-STORAGEACCOUNT-CONNECTIONSTRING");

            var eventProcessorHostName = Guid.NewGuid().ToString();
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