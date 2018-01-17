using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using VirtualService.Utils;

namespace UnitTests.Utils
{
    [TestClass]
    public class ParseContextTests
    {
        [TestMethod]
        public void ParseContext_CtorStreamReaderObject_SetsReaderAndStateProperties()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();

            // Act
            var parseContext = new ParseContext(reader, state);

            // Assert
            Assert.AreSame(reader, parseContext.Reader);
            Assert.AreSame(state, parseContext.State);

            // Cleanup
            ((IDisposable)reader).Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseContext_CtorStreamReaderObject_ThrowsOnNullStreamReader()
        {
            // Arrange
            StreamReader nullReader = null;
            var state = new object();

            // Act
            var parseContext = new ParseContext(nullReader, state);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseContext_CtorStreamReaderObject_ThrowsOnNullStateObject()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            object nullState = null;

            // Act
            var parseContext = new ParseContext(reader, nullState);

            // Cleanup
            ((IDisposable)reader).Dispose();
        }
    }
}
