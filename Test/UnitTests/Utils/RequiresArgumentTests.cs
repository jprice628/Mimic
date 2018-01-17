using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Mimic.Utils;

namespace UnitTests.Utils
{
    [TestClass]
    public class RequiresArgumentTests
    {
        [TestMethod]
        public void RequiresArgument_NotNull_DoesNothingOnNonNullValue()
        {
            // Act
            RequiresArgument.NotNull(new object(), "Value cannot be null.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresArgument_NotNull_ThrowsOnNullValue()
        {
            // Act
            RequiresArgument.NotNull(null, "Value cannot be null.");
        }

        [TestMethod]
        public void RequiresArgument_NotNullOrWhiteSpace_DoesNothingOnNonNullValue()
        {
            // Act
            RequiresArgument.NotNullOrWhiteSpace("lorem ipsum", "Value cannot be null.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RequiresArgument_NotNullOrWhiteSpace_ThrowsOnNullOrWhiteSpaceValue()
        {
            // Act
            RequiresArgument.NotNullOrWhiteSpace("   \t  ", "Value cannot be null or white space.");
        }

        [TestMethod]
        public void RequiresArgument_LengthEquals_DoesNothingOnRightSizeArray()
        {
            // Arrange
            var arr = new[] { 1, 2, 3 };

            // Act
            RequiresArgument.LengthEquals(arr, 3, "Array should contain 3 elements.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RequiresArgument_LengthEquals_ThrowsOnWrongSizeArray()
        {
            // Arrange
            var arr = new[] { 1, 2 };

            // Act
            RequiresArgument.LengthEquals(arr, 3, "Array should contain 3 elements.");
        }
    }
}
