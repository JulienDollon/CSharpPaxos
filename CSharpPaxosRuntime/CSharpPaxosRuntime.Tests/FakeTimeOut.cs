using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Environment;

namespace CSharpPaxosRuntime.Tests
{
    class FakeTimeOut : ITimeOut
    {
        private readonly int DefaultTimeOut = 1;
        private readonly int IncreaseMultiplierTimeOut = 1000;
        private readonly int MaxTimeOut = 10000;

        private int value;

        public FakeTimeOut()
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