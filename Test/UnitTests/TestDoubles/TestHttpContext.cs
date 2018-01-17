using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace UnitTests.TestDoubles
{
    public class TestHttpContext : HttpContext
    {
        private HttpRequest request;
        private HttpResponse response;

        public override IFeatureCollection Features => throw new NotImplementedException();

        public override HttpRequest Request => request;

        public override HttpResponse Response => response;

        public override ConnectionInfo Connection => throw new NotImplementedException();

        public override WebSocketManager WebSockets => throw new NotImplementedException();

        [Obsolete]
        public override AuthenticationManager Authentication => throw new NotImplementedException();

        public override ClaimsPrincipal User { get; set; }
        public override IDictionary<object, object> Items { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }
        public override string TraceIdentifier { get; set; }
        public override ISession Session { get; set; }

        public TestHttpContext()
        {
        }

        public TestHttpContext(HttpRequest request, HttpResponse response)
        {
            this.request = request;
            this.response = response;
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
