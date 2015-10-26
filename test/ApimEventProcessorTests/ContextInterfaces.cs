using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApimEventProcessorTests
{

    public interface IContext
    {
        IApi Api { get; }
        IDeployment Deployment { get; }
        IOperation Operation { get; }
        IProduct Product { get; }
        IRequest Request { get; }
        IResponse Response { get; }
        ISubscription Subscription { get; }
        bool Tracing { get; }
        IUser User { get; }
        IDictionary<string, object> Variables { get; }
    }

    public interface IApi
    {
        string Id { get; }
        string Name { get; }
        string Path { get; }
        IUrl ServiceUrl { get; }
    }

    public interface IMessageBody
    {
        T As<T>(bool preserveContent = false) where T : class; //string, JObject,JToken,JArray, XNode,XElement,XDocument
    }

    public interface IDeployment
    {
        string Region { get; }
        string ServiceName { get; }
    }

    public interface IUrl
    {
        string Host { get; }
        string Path { get; }
        int Port { get; }
        IReadOnlyDictionary<string, string[]> Query { get; }
        string QueryString { get; }
        string Scheme { get; }
    }

    public interface IOperation
    {
        string Id { get; }
        string Method { get; }
        string Name { get; }
        string UrlTemplate { get; }
    }

    public interface IProduct
    {
         IEnumerable<IApi> Apis { get; }
         bool ApprovalRequired { get; }
         IEnumerable<IGroup> Groups { get; }
         string Id { get; }
         string Name { get; }
         ProductState State { get; }
         int? SubscriptionLimit { get; }
         bool SubscriptionRequired { get; }
    }

    public interface IRequest
    {
        IMessageBody Body { get; }
        IHeaders Headers { get; }
        string Method { get; }
        IUrl Url { get; }
    }

    public interface IResponse
    {
        IMessageBody Body { get; }
        IHeaders Headers { get; }
        int StatusCode { get; }
        string StatusReason { get; }
    }

    public interface ISubscription
    {
        DateTime CreatedTime { get; }
        DateTime? EndTime { get; }
        string Id { get; }
        string Key { get; }
        string Name { get; }
        string PrimaryKey { get; }
        string SecondaryKey { get; }
        DateTime? StartDate { get; }
    }

    public class IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<IGroup> Groups { get; set; }
        public IEnumerable<IUserIdentity> Identities { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Note { get; set; }
    }

    public interface IUserIdentity
    {
        string Id { get; set; }
        string Provider { get; set; }
    }

    public class IHeaders : Dictionary<string, string[]>
    {

        public string GetValueOrDefault(string headerName, string defaultValue) { return defaultValue; }

    }

    public enum ProductState
    {
        NotPublished,
        Published
    }

    public class IGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
