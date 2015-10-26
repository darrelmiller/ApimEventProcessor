using Runscope.Links;
using Runscope.Messages;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ApimEventProcessor
{
    public class RunscopeHttpMessageProcessor : IHttpMessageProcessor
    {
        private HttpClient _HttpClient;
        private ILogger _Logger;
        private string _BucketKey;
        public RunscopeHttpMessageProcessor(HttpClient httpClient, ILogger logger)
        {
            _HttpClient = httpClient;
            var key = Environment.GetEnvironmentVariable("APIMEVENTS-RUNSCOPE-KEY", EnvironmentVariableTarget.User);
            _HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", key);
            _HttpClient.BaseAddress = new Uri("https://api.runscope.com");
            _BucketKey = Environment.GetEnvironmentVariable("APIMEVENTS-RUNSCOPE-BUCKET", EnvironmentVariableTarget.User);
            _Logger = logger;
        }

        public async Task ProcessHttpMessage(HttpMessage message)
        {
            var runscopeMessage = new RunscopeMessage()
            {
                UniqueIdentifier = message.MessageId
            };

            if (message.IsRequest)
            {
                _Logger.LogInfo("Sending HTTP request " + message.MessageId.ToString());
                runscopeMessage.Request = RunscopeRequest.CreateFromAsync(message.HttpRequestMessage).Result;
            }
            else
            {
                _Logger.LogInfo("Sending HTTP response " + message.MessageId.ToString());
                runscopeMessage.Response = RunscopeResponse.CreateFromAsync(message.HttpResponseMessage).Result;
            }

            var messagesLink = new MessagesLink() { Method = HttpMethod.Post };
            messagesLink.BucketKey = _BucketKey;
            messagesLink.RunscopeMessage = runscopeMessage;
            var runscopeResponse = await _HttpClient.SendAsync(messagesLink.CreateRequest());
            _Logger.LogDebug("Request sent to Runscope");
        }
    }
}