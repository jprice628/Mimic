using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VirtualService.Utils;

namespace UnitTests.Utils
{
    [TestClass]
    public class ParseFunctionsTests
    {
        #region ReadLine Tests

        [TestMethod]
        public void ParseFunctions_ReadLine_SetsInputAndCallsNextFunction()
        {
            // Arrange

            // Note that the string being used here has some extra white 
            // space around it.
            var buffer = Encoding.ASCII.GetBytes("  Lorem ipsum ");
            var memStream = new MemoryStream(buffer);
            var reader = new StreamReader(memStream);
            var state = new object();
            var context = new ParseContext(reader, state);

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.ReadLine(context, next).Wait();

            // Assert

            // Note that the expected value is a trimmed version of the one 
            // used above to create the buffer variable.
            Assert.AreEqual("Lorem ipsum", context.Input);
            Assert.AreEqual(1, numCallsMadeToNextFunction);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ReadLine_ThrowsOnNullParseContext()
        {
            // Arrange
            ParseContext nullParseContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.ReadLine(nullParseContext, next);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ReadLine_ThrowsOnNullNextFunction()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state);

            Func<Task> nullNextFunction = null;

            // Act
            ParseFunctions.ReadLine(context, nullNextFunction);
        }

        #endregion

        #region IgnoreCommentsAndBlankLines Tests

        [TestMethod]
        public void ParseFunctions_IgnoreCommentsAndBlankLines_CallsNextForNonCommentNonWhiteSpaceValues()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state)
            {
                Input = "Lorem ipsum"
            };

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.IgnoreCommentsAndBlankLines(context, next).Wait();

            // Assert
            Assert.AreEqual(1, numCallsMadeToNextFunction);
        }

        [TestMethod]
        public void ParseFunctions_IgnoreCommentsAndBlankLines_DoesNotCallNextOnComments()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state)
            {
                Input = "# This is a comment line."
            };

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.IgnoreCommentsAndBlankLines(context, next).Wait();

            // Assert
            Assert.AreEqual(0, numCallsMadeToNextFunction);
        }

        [TestMethod]
        public void ParseFunctions_IgnoreCommentsAndBlankLines_DoesNotCallNextOnNull()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state)
            {
                Input = null
            };

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.IgnoreCommentsAndBlankLines(context, next).Wait();

            // Assert
            Assert.AreEqual(0, numCallsMadeToNextFunction);
        }

        [TestMethod]
        public void ParseFunctions_IgnoreCommentsAndBlankLines_DoesNotCallNextOnEmptyString()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state)
            {
                Input = string.Empty
            };

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.IgnoreCommentsAndBlankLines(context, next).Wait();

            // Assert
            Assert.AreEqual(0, numCallsMadeToNextFunction);
        }

        [TestMethod]
        public void ParseFunctions_IgnoreCommentsAndBlankLines_DoesNotCallNextOnWhiteSpace()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state)
            {
                Input = "   \t   "
            };

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.IgnoreCommentsAndBlankLines(context, next).Wait();

            // Assert
            Assert.AreEqual(0, numCallsMadeToNextFunction);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_IgnoreCommentsAndBlankLines_ThrowsOnNullParseContext()
        {
            // Arrange
            ParseContext nullParseContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.IgnoreCommentsAndBlankLines(nullParseContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_IgnoreCommentsAndBlankLines_ThrowsOnNullNextFunction()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state);

            Func<Task> nullNextFunction = null;

            // Act
            ParseFunctions.IgnoreCommentsAndBlankLines(context, nullNextFunction).Wait();
        }

        #endregion

        #region ParseSetting Tests

        [TestMethod]
        public void ParseFunctions_ParseSetting_SetsFieldValueOnStateObject()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new ParseSetting_TestState();
            var context = new ParseContext(reader, state)
            {
                Input = "SettingField1: Lorem ipsum: dolor"
            };

            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.ParseSetting(context, next);

            // Assert
            Assert.AreEqual("Lorem ipsum: dolor", state.SettingField1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ParseSetting_ThrowsOnNullParseContext()
        {
            // Arrange
            ParseContext nullParseContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.ParseSetting(nullParseContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ParseSetting_ThrowsOnNullNextFunction()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state);

            Func<Task> nullNextFunction = null;

            // Act
            ParseFunctions.ParseSetting(context, nullNextFunction).Wait();
        }

        // This happens when the input does not contain at least one colon.
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseFunctions_ParseSetting_FailsWhenInputDoesNotContainANameAndValue()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new ParseSetting_TestState();
            var context = new ParseContext(reader, state)
            {
                Input = "SettingField1 Lorem ipsum"
            };

            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.ParseSetting(context, next);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ParseSetting_ThrowsWhenInputContainsAnWhiteSpaceName()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new ParseSetting_TestState();
            var context = new ParseContext(reader, state)
            {
                Input = "  \t  : Lorem ipsum"
            };

            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.ParseSetting(context, next);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ParseSetting_ThrowsWhenInputContainsAnWhiteSpaceValue()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new ParseSetting_TestState();
            var context = new ParseContext(reader, state)
            {
                Input = "SettingField1: \t   "
            };

            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.ParseSetting(context, next);
        }

        // A dummy object with a field that can be set.
        class ParseSetting_TestState
        {
            // A field that can be set during parsing.
            public string SettingField1;
        }

        #endregion

        #region ParseBody Tests

        [TestMethod]
        public void ParseFunctions_ParseBody_ParsesBodyTextWhenInputIsBodyComment()
        {
            // Arrange
            var text = new StringBuilder()
                .AppendLine("Lorem ipsum dolor sit amet, consectetur adipiscing")
                .AppendLine("elit, sed do eiusmod tempor incididunt ut labore et")
                .AppendLine("dolore magna aliqua. Ut enim ad minim veniam, quis")
                .ToString();
            var buffer = Encoding.ASCII.GetBytes(text);
            var memStream = new MemoryStream(buffer);
            var reader = new StreamReader(memStream);
            var state = new ParseBody_TestState();
            var context = new ParseContext(reader, state)
            {
                Input = "# Body"
            };

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.ParseBody(context, next).Wait();

            // Assert
            Assert.AreEqual(text, state.Body);
            Assert.AreEqual(0, numCallsMadeToNextFunction);
        }

        [TestMethod]
        public void ParseFunctions_ParseBody_CallsNextFunctionWhenInputIsNotBodyComment()
        {
            // Arrange
            var text = new StringBuilder()
                .AppendLine("Lorem ipsum dolor sit amet, consectetur adipiscing")
                .AppendLine("elit, sed do eiusmod tempor incididunt ut labore et")
                .AppendLine("dolore magna aliqua. Ut enim ad minim veniam, quis")
                .ToString();
            var buffer = Encoding.ASCII.GetBytes(text);
            var memStream = new MemoryStream(buffer);
            var reader = new StreamReader(memStream);
            var state = new ParseBody_TestState();
            var context = new ParseContext(reader, state)
            {
                Input = "Anything but # Body"
            };

            var numCallsMadeToNextFunction = 0;
            Task next()
            {
                numCallsMadeToNextFunction++;
                return Task.CompletedTask;
            }

            // Act
            ParseFunctions.ParseBody(context, next).Wait();

            // Assert
            Assert.IsNull(state.Body);
            Assert.AreEqual(1, numCallsMadeToNextFunction);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ParseBody_ThrowsOnNullParseContext()
        {
            // Arrange
            ParseContext nullParseContext = null;
            Task next() => Task.CompletedTask;

            // Act
            ParseFunctions.ParseBody(nullParseContext, next).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseFunctions_ParseBody_ThrowsOnNullNextFunction()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var state = new object();
            var context = new ParseContext(reader, state);

            Func<Task> nullNextFunction = null;

            // Act
            ParseFunctions.ParseBody(context, nullNextFunction).Wait();
        }

        // A dummy object with a field that can be set.
        class ParseBody_TestState
        {
            // A field that can be set during parsing.
            public string Body;
        }

        #endregion
    }
}
