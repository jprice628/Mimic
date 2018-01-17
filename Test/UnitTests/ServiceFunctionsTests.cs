using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;
using UnitTests.TestDoubles;
using VirtualService;

namespace UnitTests
{
    [TestClass]
    public class ServiceFunctionsTests
    {
        #region AddService Tests

        [TestMethod]
        public void ServiceFunctions_AddService_ReturnsOkAndServiceId()
        {
            // Arrange
            #region BodyString
            var bodyString = @"# BodyString
Method: GET
Path: /api/things/53";
            #endregion

            var request = new TestHttpRequest()
            {
                Method = "POST",
                Path = "/__vs/services",
                BodyString = bodyString
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.AddService(context, next).Wait();

            // Assert
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            Assert.IsTrue(Guid.TryParse(response.ReadBody(), out Guid serviceId));
            Assert.AreNotEqual(Guid.Empty, serviceId);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_AddService_ThrowsOnNullContext()
        {
            // Arrange            
            HttpContext nullContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.AddService(nullContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_AddService_ThrowsOnNullNextFunction()
        {
            // Arrange            
            HttpContext context = new TestHttpContext();
            Func<Task> nullNextFunction = null;

            // Act
            ServiceFunctions.AddService(context, nullNextFunction).Wait();
        }

        [TestMethod]
        public void ServiceFunctions_AddService_InvokesNextFunctionWhenMethodNotPOST()
        {
            // Arrange
            #region BodyString
            var bodyString = @"# BodyString
# This ""GET"" is irrelevant
Method: GET
Path: /api/things/53";
            #endregion

            var request = new TestHttpRequest()
            {
                // This "GET" causes the request not to match the required pattern.
                Method = "GET",
                Path = "/__vs/services",
                BodyString = bodyString
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);

            var numCallsToNext = 0;
            Task next()
            {
                numCallsToNext++;
                return Task.CompletedTask;
            }

            // Act
            ServiceFunctions.AddService(context, next).Wait();

            // Assert
            Assert.AreEqual(1, numCallsToNext);
        }

        [TestMethod]
        public void ServiceFunctions_AddService_InvokesNextFunctionWhenPathNotCorrect()
        {
            // Arrange
            #region BodyString
            var bodyString = @"# BodyString
Method: GET
# This path is irrelevant
Path: /api/things/53";
            #endregion

            var request = new TestHttpRequest()
            {
                Method = "POST",
                // This path causes the request not to match the required pattern.
                Path = "/__vs/somethingWrong",
                BodyString = bodyString
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);

            var numCallsToNext = 0;
            Task next()
            {
                numCallsToNext++;
                return Task.CompletedTask;
            }

            // Act
            ServiceFunctions.AddService(context, next).Wait();

            // Assert
            Assert.AreEqual(1, numCallsToNext);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_AddService_ThrowsOnMissingMethod()
        {
            // Arrange
            #region BodyString
            var bodyString = @"# BodyString
# Commenting out the Method setting should cause a failure.
# Method: GET
Path: /api/things/53";
            #endregion

            var request = new TestHttpRequest()
            {
                Method = "POST",
                Path = "/__vs/services",
                BodyString = bodyString
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.AddService(context, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_AddService_ThrowsOnMissingPath()
        {
            // Arrange
            #region BodyString
            var bodyString = @"# BodyString
Method: GET
# Commenting out the Path setting should cause a failure.
# Path: /api/things/53";
            #endregion

            var request = new TestHttpRequest()
            {
                Method = "POST",
                Path = "/__vs/services",
                BodyString = bodyString
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.AddService(context, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_AddService_ThrowsDuplicateService()
        {
            // Arrange
            #region BodyString
            var bodyString = @"# BodyString
Method: GET
Path: /api/things/53";
            #endregion

            var request = new TestHttpRequest()
            {
                Method = "POST",
                Path = "/__vs/services",
                BodyString = bodyString
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.AddService(context, next).Wait();
            ServiceFunctions.AddService(context, next).Wait();
        }

        #endregion

        #region DeleteService Tests

        [TestMethod]
        public void ServiceFunctions_DeleteService_ReturnsOk()
        {
            // Arrange
            var request = new TestHttpRequest()
            {
                Method = "DELETE",
                Path = "/__vs/services/ab61870e-793a-4c10-8427-ae57effd5c6a",
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.DeleteService(context, next).Wait();

            // Assert
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctions_DeleteService_ThrowsOnNullContext()
        {
            // Arrange
            HttpContext nullContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.DeleteService(nullContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctions_DeleteService_ThrowsOnNullNextFunction()
        {
            // Arrange
            HttpContext context = new TestHttpContext();
            Func<Task> nullNextFunction = null;

            // Act
            ServiceFunctions.DeleteService(context, nullNextFunction).Wait();
        }

        [TestMethod]
        public void ServiceFunctions_DeleteService_InvokesNextFunctionWhenMethodNotDELETE()
        {
            // Arrange
            var request = new TestHttpRequest()
            {
                // This "GET" causes the request not to match the required pattern.
                Method = "GET",
                Path = "/__vs/services/ab61870e-793a-4c10-8427-ae57effd5c6a",
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);

            var numCallsToNext = 0;
            Task next()
            {
                numCallsToNext++;
                return Task.CompletedTask;
            }

            // Act
            ServiceFunctions.DeleteService(context, next).Wait();

            // Assert
            Assert.AreEqual(1, numCallsToNext);
        }

        [TestMethod]
        public void ServiceFunctions_DeleteService_InvokesNextFunctionWhenPathNotCorrect()
        {
            // Arrange
            var request = new TestHttpRequest()
            {
                Method = "GET",
                // This path causes the request not to match the required pattern.
                Path = "/__vs/badResource/ab61870e-8427-ae57effd5c6a",
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);

            var numCallsToNext = 0;
            Task next()
            {
                numCallsToNext++;
                return Task.CompletedTask;
            }

            // Act
            ServiceFunctions.DeleteService(context, next).Wait();

            // Assert
            Assert.AreEqual(1, numCallsToNext);
        }

        [TestMethod]
        public void ServiceFunctions_DeleteServices_ReturnsNotFoundAfterDeleting()
        {
            // Arrange
            var serviceId = AddService("GET", "/api/resource/5be5223c-70d4-4a61-af5c-be5d60d9eb65");

            var request = new TestHttpRequest()
            {
                Method = "DELETE",
                Path = "/__vs/services/" + serviceId,
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.DeleteService(context, next).Wait();

            // Assert
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(404, InvokeService("GET", "/api/resource/5be5223c-70d4-4a61-af5c-be5d60d9eb65"));
        }

        #endregion

        #region DeleteAllServices Tests

        [TestMethod]
        public void ServiceFunctions_DeleteAllServices_ReturnsOk()
        {
            // Arrange
            var request = new TestHttpRequest()
            {
                Method = "DELETE",
                Path = "/__vs/services",
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.DeleteAllServices(context, next).Wait();

            // Assert
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctions_DeleteAllServices_ThrowsOnNullContext()
        {
            // Arrange
            HttpContext nullContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.DeleteAllServices(nullContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctions_DeleteAllServices_ThrowsOnNullNextFunction()
        {
            // Arrange
            var context = new TestHttpContext();
            Func<Task> nullNextFunction = null;

            // Act
            ServiceFunctions.DeleteAllServices(context, nullNextFunction).Wait();
        }

        [TestMethod]
        public void ServiceFunctions_DeleteAllServices_ReturnsNotFoundAfterDeleting()
        {
            // Arrange
            var service1Id = AddService("GET", "/api/resource/5be5223c-70d4-4a61-aa5c-be5d60d90001");
            var service2Id = AddService("GET", "/api/resource/5be5223c-70d4-4a61-aa5c-be5d60d90002");
            var service3Id = AddService("GET", "/api/resource/5be5223c-70d4-4a61-aa5c-be5d60d90003");

            var request = new TestHttpRequest()
            {
                Method = "DELETE",
                Path = "/__vs/services",
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.DeleteAllServices(context, next).Wait();

            // Assert
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(404, InvokeService("GET", "/api/resource/5be5223c-70d4-4a61-aa5c-be5d60d90001"),
                "Invoking service1 should result in a 404.");
            Assert.AreEqual(404, InvokeService("GET", "/api/resource/5be5223c-70d4-4a61-aa5c-be5d60d90002"),
                "Invoking service2 should result in a 404.");
            Assert.AreEqual(404, InvokeService("GET", "/api/resource/5be5223c-70d4-4a61-aa5c-be5d60d90003"),
                "Invoking service3 should result in a 404.");
        }

        #endregion

        #region InvokeService Tests

        [TestMethod]
        public void ServiceFunctions_InvokeService_InvokesAService()
        {
            // Arrange
            AddService("GET", "/api/things/8bd5c2be-5b8b-4e8d-97bb-cbb9ac8c5066");

            var request = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/api/things/8bd5c2be-5b8b-4e8d-97bb-cbb9ac8c5066",
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.InvokeService(context, next).Wait();

            // Assert
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("application/json", response.ContentType);
            Assert.AreEqual(@"{""color"":""Red""}", response.ReadBody());
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_InvokeService_ThrowsOnNullContext()
        {
            // Arrange            
            HttpContext nullContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.InvokeService(nullContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_InvokeService_ThrowsOnNullNextFunction()
        {
            // Arrange            
            HttpContext context = new TestHttpContext();
            Func<Task> nullNextFunction = null;

            // Act
            ServiceFunctions.InvokeService(context, nullNextFunction).Wait();
        }

        [TestMethod]
        public void ServiceFunctions_InvokeService_ReturnsNotFound()
        {
            // Arrange
            var request = new TestHttpRequest()
            {
                Method = "PUT",
                Path = "/api/things/fc845a74-973e-4db9-8951-5c99e7cf009c",
                BodyString = string.Empty
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.InvokeService(context, next).Wait();

            // Assert
            Assert.AreEqual(404, response.StatusCode);
        }

        #endregion

        #region QueryService Tests

        [TestMethod]
        public void ServiceFunctions_QueryService_ReturnsServiceStats()
        {
            // Arrange
            var serviceId = AddService("GET", "/api/serviceToBeQueried/1");
            InvokeService("GET", "/api/serviceToBeQueried/1", "Lorem ipsum dolor");
            InvokeService("GET", "/api/serviceToBeQueried/1", "sit amet, consectetur adipiscing");

            var request = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/__vs/services/" + serviceId
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.QueryService(context, next).Wait();

            // Assert
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            var expectedResponseBody = new StringBuilder()
                    .AppendLine($"CallCount: 2")
                    .AppendLine()
                    .AppendLine("LastRequestBody:")
                    .Append("sit amet, consectetur adipiscing")
                    .ToString();
            Assert.AreEqual(expectedResponseBody, response.ReadBody());
        }

        [TestMethod]
        public void ServiceFunctions_QueryService_Returns404WhenServiceDoesNotExist()
        {
            // Arrange
            var request = new TestHttpRequest()
            {
                Method = "GET",
                Path = "/__vs/services/" + Guid.NewGuid().ToString()
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.QueryService(context, next).Wait();

            // Assert
            Assert.AreEqual(404, response.StatusCode);            
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_QueryService_ThrowsOnNullContext()
        {
            // Arrange            
            HttpContext nullContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ServiceFunctions.QueryService(nullContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ServiceFunctions_QueryService_ThrowsOnNullNextFunction()
        {
            // Arrange            
            HttpContext context = new TestHttpContext();
            Func<Task> nullNextFunction = null;

            // Act
            ServiceFunctions.QueryService(context, nullNextFunction).Wait();
        }

        #endregion

        #region Helper Methods

        // Adds a service and returns it's ID.
        //
        // Do not use this to test the ServiceFunctions.AddService method. 
        // This was designed to help other tests and assumes that the 
        // ServiceFunctions.AddService has already been tested.
        private static string AddService(string method, string path)
        {
            var bodyString = @"# BodyString
# Request
Method: {0}
Path: {1}

# Response
ContentType: application/json
StatusCode: 200

# Body
{""color"":""Red""}";

            var request = new TestHttpRequest()
            {
                Method = "POST",
                Path = "/__vs/services",
                BodyString = bodyString
                    .Replace("{0}", method)
                    .Replace("{1}", path)
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;
            ServiceFunctions.AddService(context, next).Wait();
            return response.ReadBody();
        }

        // Invokes a service and returns the HTTP status code result.
        //
        // Do not use this to test the ServiceFunctions.InvokeService method. 
        // This was designed to help other tests and assumes that the 
        // ServiceFunctions.InvokeService has already been tested.
        private static int InvokeService(string method, string path, string bodyString = "")
        {
            var request = new TestHttpRequest()
            {
                Method = method,
                Path = path,
                BodyString = bodyString
            };
            var response = new TestHttpResponse();
            var context = new TestHttpContext(request, response);
            Task next() => Task.CompletedTask;

            ServiceFunctions.InvokeService(context, next).Wait();

            return response.StatusCode;
        }

        #endregion
    }
}
