using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetryBehavior
{
    public static class RetryRunner
    {
        public static void Run(Action closure, int retries = 3)
        {
            RetryRunner.RunImpl(closure, retries, 0);
        }

        private static void RunImpl(Action closure, int retries, int currentTries)
        {
            try
            {
                closure();
            }
            catch
            {
                currentTries++;

                if (currentTries == retries)
                    throw;

                RunImpl(closure, retries, currentTries);
            }
        }
    }
}
