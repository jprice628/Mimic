using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;

namespace FunctionalTests
{
    [TestClass]
    public class BasicServiceTests
    {
        private static HttpClient Http = new HttpClient();

        [TestMethod]
        public void BasicService_AddInvokeQueryAndDeleteService_ReturnsCorrectResults()
        {
            AddService();
            InvokeService();
            QueryService();
            DeleteService();
        }

        private static void AddService()
        {
            // Arrange
            #region Request Body

            var requestBody = @"# Basics
Id: 0d6a2cb8-7df9-49c7-8a5c-c1939194d9c6

# Request
Method: POST
Path: /api/students

# Response
ContentType: application/json
StatusCode: 200

# Body
{""studentId"": ""ST-001""}";

            #endregion

            // Act
            var response = Http
                .PostAsync(
                    Test.Uri("/__vs/services"),
                    new StringContent(requestBody)
                    )
                .Result;

            // Assert
            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode,
                "Adding a service should return a 200 OK response code.");
            Assert.AreEqual(
                "0d6a2cb8-7df9-49c7-8a5c-c1939194d9c6",
                response.Content.ReadAsStringAsync().Result,
                "Adding a service should return the new service's key."
                );
        }

        private static void InvokeService()
        {
            // Arrange
            #region Request Body

            var requestBody = @"{""name"": ""Sally Watson""}";

            #endregion

            // Act
            var response = Http
                .PostAsync(
                    // This URI has to match the one provided in AddService.
                    Test.Uri("/api/students"),
                    new StringContent(requestBody)
                    )
                .Result;

            // Assert
            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode,
                "Invoking the service should return a 200 OK response code.");            
            Assert.AreEqual(
                "{\"studentId\": \"ST-001\"}",
                response.Content.ReadAsStringAsync().Result,
                "When invoking the service, the body of the response should match the one provided in AddService"
                );
        }

        private static void QueryService()
        {
            // Act
            var response = Http
                // The service key in this URI comes from AddService.
                .GetAsync(Test.Uri("/__vs/services/0d6a2cb8-7df9-49c7-8a5c-c1939194d9c6"))
                .Result;

            // Assert
            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode,
                "Querying the service should return a 200 OK response code.");
            Assert.AreEqual(
                "CallCount: 1\n\nLastRequestBody:\n{\"name\": \"Sally Watson\"}",
                response.Content.ReadAsStringAsync().Result,
                "Querying the service should return the proper response body."
                );
        }

        private static void DeleteService()
        {
            // Act
            var response = Http
                // The service key in this URI comes from AddService.
                .DeleteAsync(Test.Uri("/__vs/services/0d6a2cb8-7df9-49c7-8a5c-c1939194d9c6"))
                .Result;

            // Assert
            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode,
                "Deleting the service should return a 200 OK response code.");
        }
    }
}
