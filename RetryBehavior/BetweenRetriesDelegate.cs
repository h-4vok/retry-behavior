using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetryBehavior
{
    public delegate void BetweenRetriesDelegate(int currentTry); 
}
