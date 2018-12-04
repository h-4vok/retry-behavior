using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetryBehavior.Test
{
    [TestClass]
    public class Test_RetryRunner
    {
        [TestMethod]
        public void Retry_OneExecutionIfNoException()
        {
            var number = 10;

            Action closure = () => number = number * 2;

            RetryRunner.Run(closure);

            Assert.AreEqual(20, number);
        }

        [TestMethod]
        public void Retry_TwoExecutionsIfFirstIsException()
        {
            var number = 10;
            var secondExecutionDone = false;

            Action closure = () =>
            {
                if (number == 20)
                {
                    secondExecutionDone = true;
                    return;
                }

                number *= 2;

                throw new Exception("Some exception");
            };

            RetryRunner.Run(closure);

            Assert.IsTrue(secondExecutionDone);
        }

        [TestMethod]
        public void Retry_ThreeExecutionsIfFirstTwoAreException()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;

                if (executions == 3)
                {
                    return;
                }

                throw new Exception("Holy cow");
            };

            RetryRunner.Run(closure);

            Assert.AreEqual(3, executions);
        }

        [TestMethod]
        public void Retry_ThreeExecutionsAndThenStillFailing()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;
                throw new Exception("Now this happened");
            };

            try
            {
                RetryRunner.Run(closure);
            }
            catch(Exception ex)
            {
                Assert.AreEqual(3, executions);
                Assert.AreEqual("Now this happened", ex.Message);
            }
        }

        [TestMethod]
        public void Retry_FourExecutionsAndThenStillFailing()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;
                throw new Exception("Now this happened");
            };

            try
            {
                RetryRunner.Run(closure, 4);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(4, executions);
                Assert.AreEqual("Now this happened", ex.Message);
            }
        }

        [TestMethod]
        public void RetryWithOutput_OnceAndWorked()
        {

        }
    }
}
