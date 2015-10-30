#Azure API Management Event Processor

This sample application demonstrates using the `logtoeventhub` policy in the Azure API Management service to send events containing HTTP messages to EventHub, consume those events and forward to a Runscope, a third party HTTP logging and analytics tool.

In order to run this sample you will need a number Environment variables configured with accounts and keys.

| Key Name | Purpose |
|----------|---------|
| APIMEVENTS-EVENTHUB-NAME  | Azure Event hub name configured to receive events from API Management service|
| APIMEVENTS-EVENTHUB-CONNECTIONSTRING |  Azure Event hub configuration string |
| APIMEVENTS-STORAGEACCOUNT-NAME | Azure Storage Account used for keeping track of what events have been read |
| APIMEVENTS-STORAGEACCOUNT-KEY | Key for Azure Storage Account|
| APIMEVENTS-RUNSCOPE-KEY | Application API Key for Runscope |  
| APIMEVENTS-RUNSCOPE-BUCKET | Identifier for Runscope Bucket in which messages will be stored |

The sample, as is, writes the HTTP messages to the Runscope API, however, by creating a new implementation of `IHttpMessageProcessor` it is trivial to change where the HTTP messages are relayed.
