using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetryBehavior.Test
{
    [TestClass]
    public class Test_RetryRunner
    {
        [TestMethod]
        public void Retry_OneExecutionSuccessful()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;
            };

            RetryRunner.Run(closure);

            Assert.AreEqual(1, executions);
        }

        [TestMethod]
        public void Retry_TwoExecutionsFirstFailed()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;

                if (executions != 2)
                    throw new Exception("Failed");
            };

            RetryRunner.Run(closure);

            Assert.AreEqual(2, executions);
        }

        [TestMethod]
        public void Retry_ThreeExecutionsUntilSuccess()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;

                if (executions != 3)
                    throw new Exception("Failed");
            };

            RetryRunner.Run(closure);

            Assert.AreEqual(3, executions);
        }

        [TestMethod]
        public void Retry_ThreeExecutionsAndJustFailed()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;

                throw new Exception("Failed");
            };

            try
            {
                RetryRunner.Run(closure);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failed", ex.Message);
                Assert.AreEqual(3, executions);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void Retry_FourTimesUntilSuccess()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;

                if (executions != 4)
                    throw new Exception("Failed");
            };

            RetryRunner.Run(closure, 4);

            Assert.AreEqual(4, executions);
        }

        [TestMethod]
        public void RetryWithOutput_OneSuccessfulExecution()
        {
            var executions = 0;

            Func<bool> closure = () =>
            {
                executions++;
                return true;
            };

            var output = RetryRunner.Run(closure);

            Assert.IsTrue(output);
            Assert.AreEqual(1, executions);
        }

        [TestMethod]
        public void RetryWithOutput_TwoExecutionsFirstFailed()
        {
            var executions = 0;

            bool doExecution()
            {
                executions++;

                if (executions != 2)
                    throw new Exception("Failed");

                return executions == 2;
            }

            var output = RetryRunner.Run(doExecution);

            Assert.IsTrue(output);
            Assert.AreEqual(2, executions);
        }

        [TestMethod]
        public void RetryWithOutput_ThreeExecutionsUntilSuccess()
        {
            var executions = 0;

            bool doExecution()
            {
                executions++;

                if (executions != 3)
                    throw new Exception("Failed");

                return executions == 3;
            }

            var output = RetryRunner.Run(doExecution);

            Assert.IsTrue(output);
            Assert.AreEqual(3, executions);
        }

        [TestMethod]
        public void RetryWithOutput_ThreeExecutionsAndFailed()
        {
            var executions = 0;

            bool doExecution()
            {
                executions++;

                throw new Exception("Failed");
            }

            try
            {
                var output = RetryRunner.Run(doExecution);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failed", ex.Message);
                Assert.AreEqual(3, executions);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void RetryWithOutput_FourTimesUntilSuccess()
        {
            var executions = 0;

            bool doExecution()
            {
                executions++;

                if (executions != 4)
                    throw new Exception("Failed");

                return executions == 4;
            };

            var output = RetryRunner.Run(doExecution, 4);

            Assert.AreEqual(4, executions);
        }

        [TestMethod]
        public void Retry_ZeroRetries()
        {
            var executions = 0;

            Action closure = () =>
            {
                executions++;

                throw new Exception("Failed");
            };

            try
            {
                RetryRunner.Run(closure, 0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(1, executions);
                Assert.AreEqual("Failed", ex.Message);
                return;
            }

            Assert.Fail();
        }

        public void RetryWithOutput_ZeroRetries()
        {
            var executions = 0;

            bool doExecution()
            {
                executions++;

                throw new Exception("Failed");
            }

            try
            {
                RetryRunner.Run(doExecution, 0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(1, executions);
                Assert.AreEqual("Failed", ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void RetryWithCondition_OneExecutionSuccess()
        {
            var executions = 0;

            void doExecution()
            {
                executions++;
            }

            RetryRunner.Run(doExecution, retryCondition: () => executions == 0);
        }

        [TestMethod]
        public void RetryWithCondition_ThreeExecutionsSuccess()
        {
            var executions = 0;

            void doExecution()
            {
                executions++;
            }

            RetryRunner.Run(doExecution, retryCondition: () => executions != 3);

            Assert.AreEqual(3, executions);
        }

        [TestMethod]
        public void RetryWithCondition_ThreeExecutionsFailed()
        {
            var executions = 0;

            void doExecution()
            {
                executions++;
                throw new Exception("Failed");
            }

            try
            {
                RetryRunner.Run(doExecution, retryCondition: () => executions != 3);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(3, executions);
                Assert.AreEqual("Failed", ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void RetryWithCondition_ThreeExecutionsFailedAndDoesNotSurpassRetriesCount()
        {
            var executions = 0;

            void doExecution()
            {
                executions++;
                throw new Exception("Failed");
            }

            try
            {
                RetryRunner.Run(doExecution, retryCondition: () => executions != 5);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(3, executions);
                Assert.AreEqual("Failed", ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void RetryWithCondition_ThreeExecutionsSuccessAndDoesNotSurpassRetriesCount()
        {
            var executions = 0;

            void doExecution()
            {
                executions++;
            }

            RetryRunner.Run(doExecution, retryCondition: () => executions != 5);

            Assert.AreEqual(3, executions);
        }
    }
}
