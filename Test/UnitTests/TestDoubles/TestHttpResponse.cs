using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UnitTests.TestDoubles
{
    // A dummy HttpResponse class.
    public class TestHttpResponse : HttpResponse
    {
        public override HttpContext HttpContext => throw new NotImplementedException();

        public override int StatusCode { get; set; }

        public override IHeaderDictionary Headers => throw new NotImplementedException();

        public override Stream Body { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }

        public override IResponseCookies Cookies => throw new NotImplementedException();

        public override bool HasStarted => throw new NotImplementedException();

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }

        public TestHttpResponse()
        {
            Body = new MemoryStream();
        }

        public string ReadBody()
        {
            // By the time this method is called, we've been writing, so 
            // the stream position is at the end. This moves it back to the
            // beginning, so we can read.
            Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(Body))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
