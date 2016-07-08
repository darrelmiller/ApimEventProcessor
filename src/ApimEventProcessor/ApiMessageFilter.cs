using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace ApimEventProcessor
{
    public class ApiMessageFilter : IMessageFilter
    {
        //private readonly Stopwatch configurationChangesStopWatch;
        //private readonly long updateIntervalMillis;
        //private string[] ignoredSubscriptions;
        //private string[] ignoredSucceededRequests;
        //private string[] ignoredFailedRequests;

        public ApiMessageFilter()
        {
            //ignoredSubscriptions = GetIgnoredSubscriptions();
            //ignoredSucceededRequests = GetIgnoredSucceededRequests();
            //ignoredFailedRequests = GetIgnoredSucceededRequests();

            //this.configurationChangesStopWatch = new Stopwatch();
            //this.configurationChangesStopWatch.Start();
            //updateIntervalMillis = TimeSpan.FromSeconds(5).Milliseconds;
        }

        public bool ShouldProcess(HttpMessage message)
        {
            //if (configurationChangesStopWatch.ElapsedMilliseconds > updateIntervalMillis)
            //{
            //    lock (this)
            //    {
            //        ignoredSubscriptions = GetIgnoredSubscriptions();
            //        ignoredSucceededRequests = GetIgnoredSucceededRequests();
            //        ignoredFailedRequests = GetIgnoredSucceededRequests();
            //        this.configurationChangesStopWatch.Restart();
            //    }
            //}

            //var subscriptionHeader = message.HttpRequestMessage.Headers.GetValues("ocp-apim-subscription-key").FirstOrDefault();
            //if (subscriptionHeader == null || ignoredSubscriptions.Contains(subscriptionHeader, StringComparer.OrdinalIgnoreCase))
            //{
            //    return false;
            //}

            //var requestPath = message.HttpRequestMessage.RequestUri.AbsolutePath.ToLowerInvariant();
            //if (message.HttpResponseMessage.IsSuccessStatusCode)
            //{
            //    if (ignoredSucceededRequests.Any(_ => requestPath.Contains(_)))
            //        return false;
            //}
            //else
            //{
            //    if (ignoredFailedRequests.Any(_ => requestPath.Contains(_)))
            //        return false;
            //}

            // This is to work around a weirdness in the way we had to configure the v1 endpoints in the API Management service
            if (message.HttpRequestMessage.RequestUri.OriginalString.Contains("/v1/v1/"))
            {
                message.HttpRequestMessage.RequestUri = new Uri(message.HttpRequestMessage.RequestUri.OriginalString.Replace("/v1/v1/", "/v1/"));
            }

            return true;
        }

        private static string[] GetIgnoredSubscriptions()
        {
            ConfigurationManager.RefreshSection("appSettings");
            return ConfigurationManager.AppSettings.Get("IgnoredSubscriptions").Split(',');
        }

        private static string[] GetIgnoredSucceededRequests()
        {
            ConfigurationManager.RefreshSection("appSettings");
            return ConfigurationManager.AppSettings.Get("IgnoredSucceededRequests").Split(',').Select(_ => _.ToLowerInvariant()).ToArray();
        }

        private static string[] GetIgnoredFailedRequests()
        {
            ConfigurationManager.RefreshSection("appSettings");
            return ConfigurationManager.AppSettings.Get("IgnoredFailedRequests").Split(',').Select(_ => _.ToLowerInvariant()).ToArray();
        }
    }
}