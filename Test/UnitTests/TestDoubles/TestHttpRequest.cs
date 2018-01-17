using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.TestDoubles
{
    // A dummy HttpRequest class.
    public class TestHttpRequest : HttpRequest
    {
        public override HttpContext HttpContext => throw new NotImplementedException();

        public override string Method { get; set; }
        public override string Scheme { get; set; }
        public override bool IsHttps { get; set; }
        public override HostString Host { get; set; }
        public override PathString PathBase { get; set; }
        public override PathString Path { get; set; }
        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; }
        public override string Protocol { get; set; }

        public override IHeaderDictionary Headers => throw new NotImplementedException();

        public override IRequestCookieCollection Cookies { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override Stream Body { get; set; }

        public string BodyString
        {
            set
            {
                var buffer = Encoding.ASCII.GetBytes(value);
                Body = new MemoryStream(buffer);
                Body.Seek(0, SeekOrigin.Begin);
            }
        }

        public override bool HasFormContentType => throw new NotImplementedException();

        public override IFormCollection Form { get; set; }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public TestHttpRequest()
        {
        }
    }
}
