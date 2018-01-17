using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualService.Utils;

namespace UnitTests.Utils
{
    [TestClass]
    public class RequestRequiresTests
    {
        [TestMethod]
        public void RequestRequires_NotNullOrWhiteSpace_DoesNothingOnNonNullValue()
        {
            // Act
            RequestRequires.NotNullOrWhiteSpace("lorem ipsum", "Value cannot be null.");
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void RequestRequires_NotNullOrWhiteSpace_ThrowsOnNullOrWhiteSpaceValue()
        {
            // Act
            RequestRequires.NotNullOrWhiteSpace("   \t  ", "Value cannot be null or white space.");
        }
    }
}
