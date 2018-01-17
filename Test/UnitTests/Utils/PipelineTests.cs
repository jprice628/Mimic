using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;
using Mimic.Utils;

namespace UnitTests.Utils
{
    [TestClass]
    public class PipelineTests
    {
        [TestMethod]
        public void Pipeline_Invoke_InvokesStepsInCorrectOrder()
        {
            // Arrange            
            Task step1(StringBuilder state, Func<Task> next)
            {
                state.Append("step1");
                return next();
            }

            Task step2(StringBuilder state, Func<Task> next)
            {
                state.Append("step2");
                return next();
            }

            Task step3(StringBuilder state, Func<Task> next)
            {
                state.Append("step3");
                return next();
            }

            var pipeline = new Pipeline<StringBuilder>()
                .Use(step1)
                .Use(step2)
                .Use(step3)
                .Build();

            var result = new StringBuilder();

            // Act
            pipeline.Invoke(result).Wait();

            // Assert
            Assert.AreEqual("step1step2step3", result.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Pipeline_Use_ThrowsOnNullStep()
        {
            // Arrange
            var pipeline = new Pipeline<object>();

            // Act
            pipeline.Use(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Pipeline_Build_ThrowsWhenNoStepsHaveBeenAdded()
        {
            // Arrange
            var pipeline = new Pipeline<object>();

            // Act
            pipeline.Build();
        }
    }
}
