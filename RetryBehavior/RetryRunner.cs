using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetryBehavior
{
    public static class RetryRunner
    {
        public static void Run(Action closure, int retries = 3, Func<bool?> retryCondition = null)
        {
            retryCondition = retryCondition ?? (() => null);

            RunImpl(closure, retries, 1, retryCondition);
        }

        public static void RunImpl(Action closure, int retries, int currentTries, Func<bool?> retryCondition)
        {
            void DoRetry()
            {
                currentTries++;
                RunImpl(closure, retries, currentTries, retryCondition);
            }

            try
            {
                closure();

                if (retryCondition() == true)
                {
                    DoRetry();
                }
            }
            catch
            {
                if (retries == 0 || retries == currentTries)
                {
                    throw;
                }
                else
                {
                    DoRetry();
                }
            }
        }

        public static T Run<T>(Func<T> closure, int retries = 3, Func<bool?> retryCondition = null)
        {
            T output = default(T);

            Action internalClosure = () =>
            {
                output = closure();
            };

            retryCondition = retryCondition ?? (() => null);
            RunImpl(internalClosure, retries, 1, retryCondition);

            return output;
        }
    }
}
