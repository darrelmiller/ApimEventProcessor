using Runscope.Links;
using Runscope.Messages;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ApimEventProcessor
{
    public class RunscopeHttpMessageProcessor : IHttpMessageProcessor
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;
        private readonly string bucketKey;

        public RunscopeHttpMessageProcessor(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            var key = ConfigurationManager.AppSettings.Get("APIMEVENTS-RUNSCOPE-KEY");
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", key);
            this.httpClient.BaseAddress = new Uri("https://api.runscope.com");
            bucketKey = ConfigurationManager.AppSettings.Get("APIMEVENTS-RUNSCOPE-BUCKET");
            this.logger = logger;
        }

        public async Task ProcessHttpMessage(HttpMessage message)
        {
            var runscopeMessage = new RunscopeMessage()
            {
                BucketKey = bucketKey,
                UniqueIdentifier = message.MessageId
            };

            //if (message.IsRequest)
            //{
            //    logger.LogInfo("Processing HTTP request " + message.MessageId.ToString());
                runscopeMessage.Request = await RunscopeRequest.CreateFromAsync(message.HttpRequestMessage);
            //}
            //else
            //{
            //    logger.LogWarning("Processing HTTP response " + message.MessageId.ToString());
                runscopeMessage.Response = await RunscopeResponse.CreateFromAsync(message.HttpResponseMessage);
            //}

            var messagesLink = new MessagesLink
            {
                Method = HttpMethod.Post,
                BucketKey = bucketKey,
                RunscopeMessage = runscopeMessage
            };

            var runscopeResponse = await httpClient.SendAsync(messagesLink.CreateRequest());
            if (runscopeResponse.IsSuccessStatusCode)
            {
                logger.LogDebug("Message forwarded to Runscope");
            }
            else
            {
                logger.LogDebug("Failed to send request");
            }
        }
    }
}