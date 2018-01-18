using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;

namespace FunctionalTests
{
    [TestClass]
    public class Test
    {
        private static readonly Uri BaseUri = new Uri("http://172.17.0.1:51770/");

        private static HttpClient Http = new HttpClient();

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context) => DeleteAllServices();

        [AssemblyCleanup()]
        public static void AssemblyCleanup() => DeleteAllServices();

        public static Uri Uri(string relativeUri) => new Uri(BaseUri, relativeUri);

        private static void DeleteAllServices() => 
            Http
                .DeleteAsync(Uri("/__vs/services"))
                .Wait();
    }
}
