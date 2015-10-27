using ApimEventProcessorTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApimEventProcessorTests
{
    public class PolicyTests
    {


        private static string SendRequestToEventHub(Context context)
        {
            var requestLine = string.Format("{0} {1} HTTP/1.1\r\n", 
                                                        context.Request.Method, 
                                                        context.Request.Url.Path + context.Request.Url.QueryString);

            var body = context.Request.Body?.As<string>(true);
            if (body != null && body.Length > 1024)
            {
                body = body.Substring(0, 1024);
            }

            var headers = context.Request.Headers
                                        .Select(h => string.Format("{0}: {1}", h.Key, String.Join(", ", h.Value)))
                                        .ToArray<string>();

            var headerString = (headers.Any()) ? string.Join("\r\n", headers) + "\r\n" : string.Empty;

            return "request:"   + context.Variables["message-id"] + "\n" 
                                + requestLine + headerString + "\r\n" + body; 
        }


        private static string SendResponseToEventHub(Context context)
        {
            var statusLine = string.Format("HTTP/1.1 {0} {1}\r\n", 
                                                context.Response.StatusCode, 
                                                context.Response.StatusReason);

            var body = context.Response.Body?.As<string>(true);
            if (body != null && body.Length > 1024)
            {
                body = body.Substring(0, 1024);
            }

            var headers = context.Response.Headers
                                            .Select(h => string.Format("{0}: {1}", h.Key, String.Join(", ", h.Value)))
                                            .ToArray<string>();

            var headerString = (headers.Any()) ? string.Join("\r\n", headers) + "\r\n" : string.Empty;

            return "response:"  + context.Variables["message-id"] + "\n" 
                                + statusLine + headerString + "\r\n" + body;
        }


        [Fact]
        public void RequestPolicyWithoutBody()
        {
            // Assert
            var requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://example.org/foo")
            };
            
            var variables = new Dictionary<string, object>()
            {
                { "message-id","xxxyyy"}
            };

            var context = new Context(requestMessage: requestMessage, variables: variables );

            // Act
            string policyResult = SendRequestToEventHub(context);

            //Assert
            Assert.Equal("request:xxxyyy\n" 
                                        + "GET /foo HTTP/1.1\r\n"
                                        + "Host: example.org\r\n"
                                        + "\r\n", policyResult);

        }

        [Fact]
        public void RequestPolicyWithBody()
        {
            // Assert
            var requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://example.org/foo"),
                Content = new StringContent("Hello World!")
            };

            var variables = new Dictionary<string, object>()
            {
                { "message-id","xxxyyy"}
            };

            var context = new Context(requestMessage: requestMessage, variables: variables);

            // Act
            string policyResult = SendRequestToEventHub(context);

            //Assert
            Assert.Equal("request:xxxyyy\n"
                                        + "GET /foo HTTP/1.1\r\n"
                                        + "Host: example.org\r\n"
                                        + "Content-Type: text/plain; charset=utf-8\r\n"
                                        + "Content-Length: 12\r\n"
                                        + "\r\n"
                                        + "Hello World!", policyResult);

        }


        [Fact]
        public void ResponsePolicyWithoutBody()
        {
            // Assert
            var responseMessage = new HttpResponseMessage()
            {
            };

            var variables = new Dictionary<string, object>()
            {
                { "message-id","xxxyyy"}
            };

            var context = new Context(responseMessage: responseMessage, variables: variables);

            // Act
            string policyResult = SendResponseToEventHub(context);

            //Assert
            Assert.Equal("response:xxxyyy\n"
                                        + "HTTP/1.1 200 OK\r\n"
                                        + ""
                                        + "\r\n", policyResult);

        }

        [Fact]
        public void ResponsePolicyWithBody()
        {
            // Assert
            var responseMessage = new HttpResponseMessage()
            {
                Content = new StringContent("Hello World!")
            };

            var variables = new Dictionary<string, object>()
            {
                { "message-id","xxxyyy"}
            };

            var context = new Context(responseMessage: responseMessage, variables: variables);

            // Act
            string policyResult = SendResponseToEventHub(context);

            //Assert
            Assert.Equal("response:xxxyyy\n"
                                        + "HTTP/1.1 200 OK\r\n"
                                        + "Content-Type: text/plain; charset=utf-8\r\n"
                                        + "Content-Length: 12\r\n"
                                        + "\r\n"
                                        + "Hello World!", policyResult);

        }
    }
}
