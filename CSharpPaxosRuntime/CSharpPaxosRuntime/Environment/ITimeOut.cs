using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Environment
{
    public interface ITimeOut
    {
        void ResetToDefault();
        void Increase();
        void Wait();
    }
}