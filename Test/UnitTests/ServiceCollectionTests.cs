using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VirtualService;

namespace UnitTests
{
    [TestClass]
    public class ServiceCollectionTests
    {
        [TestMethod]
        public void ServiceCollection_TryAdd_AddsAService()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource"
            };
            var service = new Service(desc);

            var serviceCollection = new ServiceCollection();

            // Act
            var result = serviceCollection.TryAdd(service);

            // Assert
            Assert.IsTrue(result, "TryAdd should return true.");
            Assert.AreEqual(1, serviceCollection.Count);
            Assert.IsTrue(serviceCollection.Contains(service), "The collection should contain the service.");
        }

        [TestMethod]
        public void ServiceCollection_TryAdd_DoesNotAddServiceWithSameId()
        {
            // Arrange
            var desc1 = new ServiceDesc()
            {
                Id = "ab61870e-793a-4c10-8427-ae57effd5c6a",
                Method = "GET",
                Path = "/api/resource"
            };
            var service1 = new Service(desc1);

            var desc2 = new ServiceDesc()
            {
                Id = "ab61870e-793a-4c10-8427-ae57effd5c6a",
                Method = "POST",
                Path = "/api/resource2"
            };
            var service2 = new Service(desc2);

            var serviceCollection = new ServiceCollection();

            // Act
            var result1 = serviceCollection.TryAdd(service1);
            var result2 = serviceCollection.TryAdd(service2);

            // Assert
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsTrue(result1, "Try add should return true when adding service1.");
            Assert.IsTrue(serviceCollection.Contains(service1), "serviceCollection should contain service1.");

            Assert.IsFalse(result2, "Try add should return false when adding service2.");
            Assert.IsFalse(serviceCollection.Contains(service2), "serviceCollection should not contain service2.");
        }

        [TestMethod]
        public void ServiceCollection_TryAdd_DoesNotAddServiceThatHandlesTheSameRequests()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource",
                BodyContains = "lorem ipsum"
            };
            var service1 = new Service(desc);
            var service2 = new Service(desc);
            var serviceCollection = new ServiceCollection();

            // Act
            var result1 = serviceCollection.TryAdd(service1);
            var result2 = serviceCollection.TryAdd(service2);

            // Assert
            Assert.AreEqual(1, serviceCollection.Count);

            Assert.IsTrue(result1, "Try add should return true when adding service1.");
            Assert.IsTrue(serviceCollection.Contains(service1), "serviceCollection should contain service1.");

            Assert.IsFalse(result2, "Try add should return false when adding service2.");
            Assert.IsFalse(serviceCollection.Contains(service2), "serviceCollection should not contain service2.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceCollection_TryAdd_ThrowsOnNullService()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Action
            serviceCollection.TryAdd(null);
        }

        [TestMethod]
        public void ServiceCollection_Count_ReturnsNumberOfServices()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource",
                BodyContains = "lorem ipsum"
            };
            var service1 = new Service(desc);
            serviceCollection.TryAdd(service1);

            desc.Method = "POST";
            var service2 = new Service(desc);
            serviceCollection.TryAdd(service2);

            desc.Method = "PUT";
            var service3 = new Service(desc);
            serviceCollection.TryAdd(service3);

            // Act
            int count = serviceCollection.Count;

            // Assert
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void ServiceCollection_TryRemove_RemovesService()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource",
                BodyContains = "lorem ipsum"
            };
            var service = new Service(desc);
            var serviceCollection = new ServiceCollection();
            serviceCollection.TryAdd(service);

            // Act
            var result = serviceCollection.TryRemove(service.Id);

            // Assert
            Assert.AreEqual(0, serviceCollection.Count);
            Assert.IsTrue(result, "TryRemove should return return.");
            Assert.IsFalse(serviceCollection.Contains(service), "serviceCollection should not contain the service.");
        }

        [TestMethod]
        public void ServiceCollection_TryRemove_ReturnsFalseWhenServiceHasAlreadyBeenRemoved()
        {
            // Arrange
            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource",
                BodyContains = "lorem ipsum"
            };
            var serviceCollection = new ServiceCollection();

            var service1 = new Service(desc);
            serviceCollection.TryAdd(service1);

            desc.Method = "POST";
            var service2 = new Service(desc);
            serviceCollection.TryAdd(service2);

            var service3 = new Service(desc);

            // Act
            var result = serviceCollection.TryRemove(service3.Id);

            // Assert
            Assert.AreEqual(2, serviceCollection.Count);
            Assert.IsFalse(result, "TryRemove should return false.");

            Assert.IsTrue(serviceCollection.Contains(service1), "serviceCollection should contain service1.");
            Assert.IsTrue(serviceCollection.Contains(service2), "serviceCollection should contain service2.");
            Assert.IsFalse(serviceCollection.Contains(service3), "serviceCollection should not contain service3.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceCollection_FirstOrDefault_ThrowsOnNullPredicate()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Action
            serviceCollection.TryGetWhere(null, out Service service);
        }

        [TestMethod]
        public void ServiceCollection_Clear_RemovesAllServices()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource",
                BodyContains = "lorem ipsum"
            };
            var service1 = new Service(desc);
            serviceCollection.TryAdd(service1);

            desc.Method = "POST";
            var service2 = new Service(desc);
            serviceCollection.TryAdd(service2);

            desc.Method = "PUT";
            var service3 = new Service(desc);
            serviceCollection.TryAdd(service3);

            // Act
            serviceCollection.Clear();

            // Assert
            Assert.AreEqual(0, serviceCollection.Count);
            Assert.IsFalse(serviceCollection.Contains(service1), "serviceCollection should not contain service1.");
            Assert.IsFalse(serviceCollection.Contains(service2), "serviceCollection should not contain service2.");
            Assert.IsFalse(serviceCollection.Contains(service3), "serviceCollection should not contain service3.");
        }

        [TestMethod]
        public void ServiceCollection_Contains_ReturnsTrueWhenServiceExists()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            var desc = new ServiceDesc()
            {
                Method = "GET",
                Path = "/api/resource",
                BodyContains = "lorem ipsum"
            };
            var service1 = new Service(desc);
            serviceCollection.TryAdd(service1);

            // Act
            var result = serviceCollection.Contains(service1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceCollection_Contains_ThrowsOnNullService()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.Contains(null);
        }

        [TestMethod]
        public void ServiceCollection_TryGetById_ReturnsServiceWithSpecifiedId()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var desc = new ServiceDesc()
            {
                Id = serviceId.ToString(),
                Method = "GET",
                Path = "/api/resource"
            };
            var newService = new Service(desc);
            var serviceCollection = new ServiceCollection();
            serviceCollection.TryAdd(newService);

            // Act
            var result = serviceCollection.TryGetById(serviceId, out Service foundService);

            // Assert
            Assert.IsTrue(result);
            Assert.AreSame(newService, foundService);
        }

        [TestMethod]
        public void ServiceCollection_TryGetById_ReturnsFalseWhenServiceNotFound()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            
            // Act
            var result = serviceCollection.TryGetById(Guid.NewGuid(), out Service foundService);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(foundService);
        }
    }
}
