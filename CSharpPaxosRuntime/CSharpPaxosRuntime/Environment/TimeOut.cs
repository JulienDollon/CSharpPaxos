using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Environment
{
    public class TimeOut
    {
        private readonly int DefaultTimeOut = 1;
        private readonly int IncreaseMultiplierTimeOut = 2;
        private readonly int MaxTimeOut = 200;

        private int value;

        public TimeOut()
        {
            ResetToDefault();
        }

        public void ResetToDefault()
        {
            value = DefaultTimeOut;
        }

        public void Increase()
        {
            if (value * IncreaseMultiplierTimeOut <= MaxTimeOut)
            {
                value *= IncreaseMultiplierTimeOut;
            }
        }

        public void Wait()
        {
            Thread.Sleep(value);
        }
    }
}