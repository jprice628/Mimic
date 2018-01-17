using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnitTests.TestDoubles;
using Mimic;

namespace UnitTests
{
    [TestClass]
    public class ServiceTests
    {
        #region CtorServiceDesc Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_CtorServiceDesc_ThrowsOnNullMethod()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = null,
                Path = "api/resource"
            };

            // Act
            new Service(desc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_CtorServiceDesc_ThrowsOnNullPath()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = null
            };

            // Act
            new Service(desc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_CtorServiceDesc_ThrowsOnNullContentType()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                ContentType = null
            };

            // Act
            new Service(desc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_CtorServiceDesc_ThrowsOnNullStatusCode()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                StatusCode = null
            };

            // Act
            new Service(desc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_CtorServiceDesc_ThrowsOnNullBody()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                Body = null
            };

            // Act
            new Service(desc);
        }

        [TestMethod]
        public void Service_CtorServiceDesc_GeneratesIdWhenNotProvided()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Id = null,
                Method = "GET",
                Path = "api/resource"
            };

            // Act
            var service = new Service(desc);

            // Assert
            Assert.AreNotEqual(Guid.Empty, service.Id);
        }

        [TestMethod]
        public void Service_CtorServiceDesc_UsesIdWhenProvided()
        {
            // Arrange
            var id = Guid.NewGuid();
            var desc = new ServiceDesc()
            {
                Id = id.ToString(),
                Method = "GET",
                Path = "api/resource"
            };

            // Act
            var service = new Service(desc);

            // Assert
            Assert.AreEqual(id, service.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Service_CtorServiceDesc_ThrowsOnInvalidId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var desc = new ServiceDesc()
            {
                Id = "Not a valid ID.",
                Method = "GET",
                Path = "api/resource"
            };

            // Act
            new Service(desc);
        }

        #endregion

        #region HandlesSameRequests Tests

        [TestMethod]
        public void Service_HandlesSameRequests_ReturnsTrueWhenMatchingMethodPathAndBodyFilter()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                BodyContains = "bodyFilter"
            };

            var service1 = new Service(desc);
            var service2 = new Service(desc);

            // Act
            var result = service1.HandlesSameRequests(service2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Service_HandlesSameRequests_ReturnsFalseWhenMismatchedMethod()
        {
            // Arrange
            var desc1 = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                BodyContains = "bodyFilter"
            };
            var service1 = new Service(desc1);

            var desc2 = new ServiceDesc()
            {
                Method = "POST",
                Path = "api/resource",
                BodyContains = "bodyFilter"
            };
            var service2 = new Service(desc2);

            // Act
            var result = service1.HandlesSameRequests(service2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Service_HandlesSameRequests_ReturnsFalseWhenMismatchedPath()
        {
            // Arrange
            var desc1 = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource1",
                BodyContains = "bodyFilter"
            };
            var service1 = new Service(desc1);

            var desc2 = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource2",
                BodyContains = "bodyFilter"
            };
            var service2 = new Service(desc2);

            // Act
            var result = service1.HandlesSameRequests(service2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Service_HandlesSameRequests_ReturnsFalseWhenMismatchedBodyFilter()
        {
            // Arrange
            var desc1 = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                BodyContains = "Lorem ipsum"
            };
            var service1 = new Service(desc1);

            var desc2 = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                BodyContains = "dolor sit"
            };
            var service2 = new Service(desc2);

            // Act
            var result = service1.HandlesSameRequests(service2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_HandlesSameRequests_ThrowsOnNullService()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource",
                BodyContains = "Lorem ipsum"
            };
            var service = new Service(desc);

            // Act
            service.HandlesSameRequests(null);
        }

        #endregion

        #region MatchesRequest Tests

        [TestMethod]
        public void Service_MatchesRequest_ReturnsTrueWhenMatchesWithoutBodyFilter()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                // Note: case shouldn't matter. The ctor converts it to uppercase.
                Method = "get",
                // Note: case shouldn't matter. When matching both the sides are converted to uppercase. 
                Path = "/API/Resource?X=1",
            };
            var service = new Service(desc);

            var httpRequest = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/api/resource",
                QueryString = new QueryString("?x=1")
            };

            // Act
            var result = service.MatchesRequest(httpRequest, string.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Service_MatchesRequest_ReturnsTrueWhenMatchesWithBodyFilter()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                // Note: case shouldn't matter. The ctor converts it to uppercase.
                Method = "get",
                // Note: case shouldn't matter. When matching both the sides are converted to uppercase. 
                Path = "/API/Resource?X=1",
                // Case matters on the body filter.
                BodyContains = "dolor sit"
            };
            var service = new Service(desc);

            var httpRequest = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/api/resource",
                QueryString = new QueryString("?x=1")
            };

            // Act
            var result = service.MatchesRequest(
                httpRequest, 
                "Lorem ipsum dolor sit amet, consectetur adipiscing"
                );

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Service_MatchesRequest_ReturnsFalseWhenMismatchedMethod()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                // Note: case shouldn't matter. The ctor converts it to uppercase.
                Method = "POST",
                // Note: case shouldn't matter. When matching both the sides are converted to uppercase. 
                Path = "/API/Resource?X=1",
                // Case matters on the body filter.
                BodyContains = "dolor sit"
            };
            var service = new Service(desc);

            var httpRequest = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/api/resource",
                QueryString = new QueryString("?x=1")
            };

            // Act
            var result = service.MatchesRequest(
                httpRequest,
                "Lorem ipsum dolor sit amet, consectetur adipiscing"
                );

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Service_MatchesRequest_ReturnsFalseWhenMismatchedPath()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                // Note: case shouldn't matter. The ctor converts it to uppercase.
                Method = "GET",
                // Note: case shouldn't matter. When matching both the sides are converted to uppercase. 
                Path = "/api/wrongpath?X=1",
                // Case matters on the body filter.
                BodyContains = "dolor sit"
            };
            var service = new Service(desc);

            var httpRequest = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/api/resource",
                QueryString = new QueryString("?x=1")
            };

            // Act
            var result = service.MatchesRequest(
                httpRequest,
                "Lorem ipsum dolor sit amet, consectetur adipiscing"
                );

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Service_MatchesRequest_ReturnsFalseWhenMismatchedBodyFilter()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                // Note: case shouldn't matter. The ctor converts it to uppercase.
                Method = "GET",
                // Note: case shouldn't matter. When matching both the sides are converted to uppercase. 
                Path = "/api/resource?x=1",
                // Case matters on the body filter.
                BodyContains = "This isn't in the body."
            };
            var service = new Service(desc);

            var httpRequest = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/api/resource",
                QueryString = new QueryString("?x=1")
            };

            // Act
            var result = service.MatchesRequest(
                httpRequest,
                "Lorem ipsum dolor sit amet, consectetur adipiscing"
                );

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_MatchesRequest_ThrowsOnNullRequest()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/API/Resource?X=1",
            };
            var service = new Service(desc);

            // Act
            var result = service.MatchesRequest(null, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_MatchesRequest_ThrowsOnNullBody()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/API/Resource?X=1",
            };
            var service = new Service(desc);

            var httpRequest = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/api/resource",
                QueryString = new QueryString("?x=1")
            };

            // Act
            var result = service.MatchesRequest(httpRequest, null);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region WriteResponseTo Tests

        [TestMethod]
        public void Service_WriteResponseTo_WritesResponse()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource?x=1",
                ContentType = "application/json",
                StatusCode = "200",
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing"
            };
            var service = new Service(desc);

            var httpResponse = new TestHttpResponse();

            // Act
            service.WriteResponseTo(httpResponse).Wait();

            // Assert
            Assert.AreEqual(200, httpResponse.StatusCode);
            Assert.AreEqual("application/json", httpResponse.ContentType);
            Assert.AreEqual(
                "Lorem ipsum dolor sit amet, consectetur adipiscing",
                httpResponse.ReadBody()
                );
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void Service_WriteResponseTo_ThrowsOnNullResponse()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource?x=1",
                ContentType = "application/json",
                StatusCode = "200",
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing"
            };
            var service = new Service(desc);

            // Act
            service.WriteResponseTo(null).Wait();
        }

        #endregion

        #region RecordCall Tests

        [TestMethod]
        public void Service_RecordCall_IncrementsCallCountAndSavesBody()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource"
            };
            var service = new Service(desc);
            var httpRequestBody = "Lorem ipsum dolor sit amet, consectetur adipiscing";

            // Act
            service.RecordCall(httpRequestBody);
            var stats = service.Stats;

            // Assert
            Assert.AreEqual(1, stats.CallCount);
            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing", stats.LastRequestBody);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Service_RecordCall_ThrowsOnNullRequestBody()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "api/resource"
            };
            var service = new Service(desc);

            // Act
            service.RecordCall(null);
        }

        #endregion
    }
}
