using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApimEventProcessorTests
{
    public class Context : IContext
    {
        public IApi Api { get;  }
        public IDeployment Deployment { get; }
        public IOperation Operation { get; }
        public IProduct Product { get; }
        public IRequest Request { get; }
        public IResponse Response { get; }
        public ISubscription Subscription { get; }
        public bool Tracing { get; }
        public IUser User { get; }
        public IDictionary<string,object> Variables { get; }
        void Trace(string message) { }
        public Context( HttpRequestMessage requestMessage = null,
                        HttpResponseMessage responseMessage = null,
                        IDictionary<string,object> variables = null)
        {
            Api = new Api();
            Deployment = new Deployment();
            Operation = new Operation();
            Product = new Product();
            Request =  new Request(requestMessage);
            Response = new Response(responseMessage);
            Subscription = new Subscription();

            Variables = variables ?? new Dictionary<string, object>();

 
        }
    }


    internal class Subscription : ISubscription
    {
        public DateTime CreatedTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? EndTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Key
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PrimaryKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string SecondaryKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? StartDate
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class Api : IApi
    {
        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Path
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IUrl ServiceUrl
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class Product : IProduct
    {
        public IEnumerable<IApi> Apis
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ApprovalRequired
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IGroup> Groups
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ProductState State
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int? SubscriptionLimit
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool SubscriptionRequired
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }


    internal class Deployment : IDeployment
    {
        public string Region
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ServiceName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class Operation : IOperation
    {
        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Method
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string UrlTemplate
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class Response : IResponse
    {
        private HttpResponseMessage _Response;
        private IHeaders _Headers;
        private MessageBody _Body;

        public Response(HttpResponseMessage response)
        {
            _Response = response;
            _Headers = new IHeaders();
            if (response != null)
            {
                foreach (var h in response.Headers)
                {
                    _Headers.Add(h.Key, h.Value.ToArray());
                }
                if (response.Content != null)
                {
                    foreach (var h in response.Content.Headers)
                    {
                        _Headers.Add(h.Key, h.Value.ToArray());
                    }
                    _Headers.Add("Content-Length", new string[] { response.Content.Headers.ContentLength.ToString() });
                    _Body = new MessageBody(response.Content);
                }
            }
        }

        public IMessageBody Body
        {
            get { return _Body; }
        }

        public IHeaders Headers
        {
            get { return _Headers; }
        }

        public int StatusCode
        {
            get { return (int)_Response.StatusCode; }
        }

        public string StatusReason
        {
            get { return _Response.ReasonPhrase;}
        }
    }

    internal class Request : IRequest
    {
        private Url _Url;
        private HttpRequestMessage _Request;
        private IHeaders _Headers;
        private MessageBody _Body;
        public Request(HttpRequestMessage request)
        {
            if (request != null)
            {
                _Url = new Url(request.RequestUri);
            }
            _Request = request;
            _Headers = new IHeaders();

            if (request != null)
            {
                _Headers.Add("Host", new string[] { _Request.RequestUri.Host });
                foreach (var h in request.Headers)
                {
                    _Headers.Add(h.Key, h.Value.ToArray());
                }
                if (request.Content != null)
                {
                    foreach (var h in request.Content.Headers)
                    {
                        _Headers.Add(h.Key, h.Value.ToArray());
                    }
                    _Headers.Add("Content-Length", new string[] { request.Content.Headers.ContentLength.ToString() });
                    _Body = new MessageBody(request.Content);
                }
            }
            
        }

        public IMessageBody Body
        {
            get { return _Body; }
        }

        public IHeaders Headers
        {
            get { return _Headers; }
        }

        public string Method
        {
            get { return _Request.Method.ToString(); }
        }

        public IUrl Url
        {
            get { return _Url; }
        }
    }

    internal class MessageBody : IMessageBody
    {
        private HttpContent _Content;
        public MessageBody(HttpContent content)
        {
            _Content = content;
        }
        public T As<T>(bool preserveContent = false) where T : class
        {
            return _Content.ReadAsStringAsync().Result as T;         
        }
    }
    internal class Url : IUrl
    {
        private Uri _Uri;

        public Url(Uri uri)  
        {
            _Uri = uri;
        }

        public string Host
        {
            get { return _Uri.Host;}
        }

        public string Path
        {
            get { return _Uri.AbsolutePath; }
        }

        public int Port
        {
            get { return _Uri.Port; }
        }

        public IReadOnlyDictionary<string, string[]> Query
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string QueryString
        {
            get { return _Uri.Query; }

        }

        public string Scheme
        {
            get { return _Uri.Scheme; }
        }
    } 

  

}
