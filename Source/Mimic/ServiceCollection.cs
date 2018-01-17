using System;
using System.Collections.Generic;
using System.Linq;
using Mimic.Utils;

namespace Mimic
{
    /// <summary>
    /// Provides a thread-safe collection for services with specialized 
    /// methods to support the ServiceFunctions.
    /// </summary>
    public class ServiceCollection
    {
        private object thisLock;
        private List<Service> internalCollection;

        public int Count
        {
            get
            {
                lock(thisLock)
                {
                    return internalCollection.Count;
                }
            }
        }

        public ServiceCollection()
        {
            thisLock = new object();
            internalCollection = new List<Service>();
        }

        /// <summary>
        /// Attempts to add a service to the collection. Returns true if the 
        /// service is added. Returns false when another service either has 
        /// the same ID or handles the same requests.
        /// </summary>
        public bool TryAdd(Service service)
        {
            RequiresArgument.NotNull(service, "service");

            lock(thisLock)
            {
                if (internalCollection.Any(x => 
                    x.Id == service.Id || 
                    x.HandlesSameRequests(service)
                    ))
                {
                    return false;
                }                
                else
                {
                    internalCollection.Add(service);
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets a service by ID. Returns true if the service exists and the 
        /// out parameter is set; otherwise, return false.
        /// </summary>
        public bool TryGetById(Guid id, out Service service)
        {
            lock(thisLock)
            {
                service = internalCollection.FirstOrDefault(x => x.Id == id);
                return service != null;
            }
        }

        /// <summary>
        /// Removes a service by ID. Returns true if the service is 
        /// removed; otherwise, false.
        /// </summary>
        public bool TryRemove(Guid id)
        {
            lock(thisLock)
            {
                return internalCollection.RemoveAll(x => x.Id == id) > 0;
            }
        }

        /// <summary>
        /// Removes all services from the collection.
        /// </summary>
        public void Clear()
        {
            lock (thisLock)
            {
                internalCollection.Clear();
            }
        }

        /// <summary>
        /// Returns true if the collection contains the specified service.
        /// </summary>
        public bool Contains(Service service)
        {
            RequiresArgument.NotNull(service, "service");

            lock (thisLock)
            {
                return internalCollection.Contains(service);
            }
        }

        /// <summary>
        /// Gets the first service in the collection that matched the 
        /// specified predicate. Returns true if a service is found; 
        /// otherwise false.
        /// </summary>
        public bool TryGetWhere(Func<Service, bool> predicate, out Service service)
        {
            RequiresArgument.NotNull(predicate, "predicate");

            lock(thisLock)
            {
                service = internalCollection.FirstOrDefault(predicate);
                return service != null;
            }
        }
    }
}
