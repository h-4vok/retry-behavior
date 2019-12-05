using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RetryBehavior
{
    public static class RetryRunner
    {
        internal static Func<bool?> GetFuncOrDefault(Func<bool?> funcInstance) =>
            funcInstance ?? (() => null);

        internal static BetweenRetriesDelegate GetDelegateOrDefault(BetweenRetriesDelegate delegateInstance) =>
            delegateInstance ?? new BetweenRetriesDelegate(x => { });

        /// <summary>
        /// Runs the given closure at least once, or retries as many times specified, sleeping for the given timespan between executions.
        /// </summary>
        /// <param name="closure">The closure to run.</param>
        /// <param name="betweenRetriesSpan">A TimeSpan of time to sleep between executions (if needed to retry).</param>
        /// <param name="retries">Times to retry the closure before allowing the exception to bubble up, if any.</param>
        /// <param name="retryCondition">Optional condition to check before an execution retry is performed.</param>
        public static void Run(Action closure, TimeSpan betweenRetriesSpan, int retries = 3, Func<bool?> retryCondition = null)
        {
            var betweenRetriesDelegate = new BetweenRetriesDelegate(x => Thread.Sleep(betweenRetriesSpan));

            RetryRunner.Run(closure, retries, retryCondition, betweenRetriesDelegate);
        }

        /// <summary>
        /// Runs the given closure at least once, or retries as many times specified.
        /// </summary>
        /// <param name="closure">The closure to run.</param>
        /// <param name="retriesAfterException">Times to retry the closure before allowing the exception to bubble up, if any.</param>
        /// <param name="retryConditionWhenNoException">Optional condition to check before an execution retry is performed.</param>
        /// <param name="betweenRetriesDelegate">Optional action delegate to execute between the closure´s executions.</param>
        public static void Run(Action closure, int retriesAfterException = 3, Func<bool?> retryConditionWhenNoException = null, BetweenRetriesDelegate betweenRetriesDelegate = null)
        {
            // By default, no condition in order to retry
            retryConditionWhenNoException = GetFuncOrDefault(retryConditionWhenNoException);

            // By default, nothing to do between retries
            betweenRetriesDelegate = GetDelegateOrDefault(betweenRetriesDelegate);

            RunImpl(closure, retriesAfterException, 1, retryConditionWhenNoException, betweenRetriesDelegate);
        }

        internal static void RunImpl(Action closure, int retriesAfterException, int currentTries, Func<bool?> retryConditionWhenNoException, BetweenRetriesDelegate betweenRetriesDelegate)
        {
            void DoRetry()
            {
                currentTries++;
                RunImpl(closure, retriesAfterException, currentTries, retryConditionWhenNoException, betweenRetriesDelegate);
            }

            try
            {
                closure();

                if (retryConditionWhenNoException() == true)
                {
                    betweenRetriesDelegate(currentTries);
                    DoRetry();
                }
            }
            catch
            {
                if (retriesAfterException == 0 || retriesAfterException == currentTries)
                {
                    throw;
                }
                else
                {
                    betweenRetriesDelegate(currentTries);
                    DoRetry();
                }
            }
        }

        /// <summary>
        /// Runs the given closure at least once, or retries as many times specified.
        /// </summary>
        /// /// <typeparam name="T">The type of the closure´s output.</typeparam>
        /// <param name="closure">The closure to run.</param>
        /// <param name="retriesAfterException">Times to retry the closure before allowing the exception to bubble up, if any.</param>
        /// <param name="retryConditionWhenNoException">Optional condition to check before an execution retry is performed.</param>
        /// <param name="betweenRetriesDelegate">Optional action delegate to execute between the closure´s executions.</param>
        /// <returns>The output of the specified closure.</returns>
        public static T Run<T>(Func<T> closure, int retriesAfterException = 3, Func<bool?> retryConditionWhenNoException = null, BetweenRetriesDelegate betweenRetriesDelegate = null)
        {
            T output = default(T);

            void internalClosure()
            {
                output = closure();
            }

            // By default, no condition in order to retry
            retryConditionWhenNoException = GetFuncOrDefault(retryConditionWhenNoException);

            // By default, nothing to do between retries
            betweenRetriesDelegate = GetDelegateOrDefault(betweenRetriesDelegate);

            RunImpl(internalClosure, retriesAfterException, 1, retryConditionWhenNoException, betweenRetriesDelegate);

            return output;
        }

        /// <summary>
        /// Runs the given closure at least once, or retries as many times specified, sleeping for the given timespan between executions.
        /// </summary>
        /// <typeparam name="T">The type of the closure´s output.</typeparam>
        /// <param name="closure">The closure to run.</param>
        /// <param name="betweenRetriesSpan">A TimeSpan of time to sleep between executions (if needed to retry).</param>
        /// <param name="retriesAfterException">Times to retry the closure before allowing the exception to bubble up, if any.</param>
        /// <param name="retryConditionWhenNoException">Optional condition to check before an execution retry is performed.</param>
        /// <returns>The output of the specified closure.</returns>
        public static T Run<T>(Func<T> closure, TimeSpan betweenRetriesSpan, int retriesAfterException = 3, Func<bool?> retryConditionWhenNoException = null)
        {
            var betweenRetriesDelegate = new BetweenRetriesDelegate(x => Thread.Sleep(betweenRetriesSpan));

            return RetryRunner.Run(closure, retriesAfterException, retryConditionWhenNoException, betweenRetriesDelegate);
        }
    }
}
