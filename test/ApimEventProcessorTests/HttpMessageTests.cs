using ApimEventProcessor;
using ApimEventProcessorTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApimEventProcessorTests
{
    public class HttpMessageTests
    {
        [Fact]
        public void ParseRequestMessage()
        {
            var message = HttpMessage.Parse("request:fa464f02-8c02-4ee3-a8ca-66301688b5c4\n" + 
                "GET /foo HTTP/1.1\r\n" + 
                "Host: example.org\r\n" + 
                "\r\n");

            Assert.NotNull(message);

            Assert.NotNull(message.HttpRequestMessage);
            Assert.True(message.IsRequest);
            Assert.Equal("http://example.org/foo", message.HttpRequestMessage.RequestUri.AbsoluteUri);
            Assert.Equal("fa464f02-8c02-4ee3-a8ca-66301688b5c4", message.MessageId.ToString());
            Assert.Equal(HttpMethod.Get, message.HttpRequestMessage.Method);
        }


        [Fact]
        public async Task ParseResponseMessage()
        {
            var message = HttpMessage.Parse("response:fa464f02-8c02-4ee3-a8ca-66301688b5c4\n" +
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain\r\n" +
                "Content-Length: 12\r\n" +
                "\r\n" + 
                "Hello World!");

            Assert.NotNull(message);

            Assert.NotNull(message.HttpResponseMessage);
            Assert.False(message.IsRequest);
            Assert.Equal("fa464f02-8c02-4ee3-a8ca-66301688b5c4", message.MessageId.ToString());
            Assert.Equal(HttpStatusCode.OK, message.HttpResponseMessage.StatusCode);
            Assert.Equal("Hello World!", await message.HttpResponseMessage.Content.ReadAsStringAsync());

        }


    }
}
